module BetterTTD.Shell

open System
open BetterTTD.OpenTTDModule
open BetterTTD.PacketTransformers
open Elmish
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.Components.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.DSL

type Model =
    { OpenTTD : OpenTTD option
      Login   : Login.Model
      Home    : Home.Model }

type Msg =
    | LoginMsg of Login.Msg
    | HomeMsg of Home.Msg

let init =
    let login, loginCmd = Login.init ()
    let home, homeCmd = Home.init ()
    { OpenTTD = None
      Login = login
      Home = home },
    Cmd.batch [ Cmd.map LoginMsg loginCmd
                Cmd.map HomeMsg  homeCmd ]

let subscribe (ottd : OpenTTD) =
    let sub dispatch =
        ottd.OnUpdate.Publish
        |> Observable.subscribe (fun pac -> dispatch (Home.Msg.ServerUpdate pac))
        |> ignore
    Cmd.ofSub sub

let update (msg : Msg) (model : Model) =
    match msg with
    | LoginMsg loginMsg ->
        let loginModel, loginCmd, extraMsg = Login.update loginMsg model.Login
        
        let newModel, newCmd =
            match extraMsg with
            | Login.ExternalMsg.NoOp -> model, Cmd.none
            | Login.ExternalMsg.Connected (ottd, welcomeMsg) ->
                let welcome = Home.Msg.ServerUpdate <| ServerWelcome welcomeMsg
                let refreshClients = Home.Msg.RefreshClients
                { model with OpenTTD = Some ottd },
                Cmd.batch [ subscribe ottd
                            Cmd.ofMsg welcome
                            Cmd.ofMsg refreshClients ]
                
        { newModel with Login = loginModel },
        Cmd.batch [ Cmd.map LoginMsg loginCmd
                    Cmd.map HomeMsg newCmd ]
        
    | HomeMsg homeMsg ->
        let homeModel, homeCmd, extraMsg = Home.update homeMsg model.Home
        
        match extraMsg with
        | Home.ExternalMsg.NoOp -> ()
        | Home.ExternalMsg.PollClient clientId ->
            match model.OpenTTD with
            | Some ottd -> ottd.AskPollClient clientId
            | None -> failwith "Invalid operation"
        
        { model with Home = homeModel },
        Cmd.map HomeMsg homeCmd

let view (model : Model) dispatch =
    match model.OpenTTD with
    | None   -> (Login.view model.Login (LoginMsg >> dispatch))
    | Some _ -> (Home.view model.Home (HomeMsg >> dispatch))
        
type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title     <- "BetterTTD - FOAN"
        base.Width     <- 800.0
        base.Height    <- 600.0
        base.MinWidth  <- 800.0
        base.MinHeight <- 600.0
        base.TransparencyLevelHint <- WindowTransparencyLevel.None

        Elmish.Program.mkProgram (fun () -> init) update view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.run