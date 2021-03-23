module BetterTTD.Home

open System
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open BetterTTD.Network.Enums
open BetterTTD.PacketTransformers
open Elmish

type Client =
    { ClientID : uint32
      Address  : string option
      Name     : string option
      Language : NetworkLanguage option }
    with
        static member create id =
            { ClientID = id
              Address  = None
              Name     = None
              Language = None }
type ChatMessage =
    { Sender  : string
      Message : string }

type Model =
    { Clients  : Client list
      Messages : ChatMessage list }

type ExternalMsg =
    | NoOp
    | PollClient   of uint32

type Msg =
    | ServerUpdate of PacketMessage
    | RefreshClients
    
let remove (clients : Client list) clientId =
    clients |> List.filter (fun x -> x.ClientID <> clientId)
    
let addOrUpdate (clients : Client list) (client : Client) =
    if clients |> List.exists (fun x -> x.ClientID = client.ClientID) then
        remove clients client.ClientID @ [ client ]
    else 
        clients @ [ client ]

let handleUpdate (model : Model) = function
    | ServerWelcome      welcome ->
        printfn "welcome %A" welcome; model, NoOp
        
    | ServerChat         chat    ->
        printfn "chat %A" chat
        let clientName =
            model.Clients
            |> List.tryFind (fun x -> x.ClientID = chat.ClientID)
            |> Option.bind (fun x -> x.Name)
            |> Option.defaultValue "Unknown"
        let chatMsg = { Sender = clientName; Message = chat.Message }
        { model with Messages = model.Messages @ [ chatMsg ] }, NoOp
        
    | ServerClientJoin   client  ->
        printfn "join %A" client
        let newClient = Client.create client.ClientID
        { model with Clients = addOrUpdate model.Clients newClient }, PollClient client.ClientID
        
    | ServerClientInfo   client  ->
        printfn "info %A" client
        let newClient = { Client.create client.ClientID with
                            Address  = Some client.Address
                            Name     = Some client.Name
                            Language = Some client.Language }
        { model with Clients = addOrUpdate model.Clients newClient }, NoOp
        
    | ServerClientUpdate client  ->
        printfn "update %A" client
        let newClient = Client.create client.ClientID
        { model with Clients = addOrUpdate model.Clients newClient }, NoOp
        
    | ServerClientError  client  ->
        printfn "error %A" client
        { model with Clients = remove model.Clients client.ClientID }, NoOp
        
    | ServerClientQuit   client  ->
        printfn "quit %A" client
        { model with Clients = remove model.Clients client.ClientID }, NoOp
        
    | _ -> model, NoOp

let init () =
    { Clients  = List.empty
      Messages = List.empty },
    Cmd.none
    
let update msg model =
    match msg with
    | ServerUpdate upd ->
        let newModel, externalMsg = handleUpdate model upd
        newModel, Cmd.none, externalMsg
    | RefreshClients ->
        model, Cmd.none, PollClient UInt32.MaxValue

let chat messages =
    if List.isEmpty messages then "Chat is empty..."
    else messages
         |> List.map (fun x -> sprintf $"[{x.Sender}] {x.Message}\n")
         |> List.reduce (+)

let navigation =
    TabControl.create [
        
    ]

let view (model : Model) dispatch =
    Grid.create [
         Grid.children [
             StackPanel.create [
                 StackPanel.verticalAlignment VerticalAlignment.Center
                 StackPanel.horizontalAlignment HorizontalAlignment.Center
                 StackPanel.children [
                     TextBlock.create [
                         TextBlock.text <| chat model.Messages
                     ]
                     Button.create [
                         Button.onClick (fun _ -> dispatch RefreshClients)
                         Button.content "Refresh Clients"
                     ]
                     TextBlock.create [
                         TextBlock.text <| model.Clients.Length.ToString ()
                     ]
                 ]
             ]
         ]
     ]