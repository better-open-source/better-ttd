module BetterTTD.Login

open System.Net
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open BetterTTD.OpenTTDModule
open BetterTTD.PacketTransformers
open Elmish

type Model =
    { Host : string
      Port : int
      Pass : string }

type ExternalMsg =
    | NoOp
    | Connected of OpenTTD * ServerWelcomeMessage
    
type Msg =
    | HostChanged of string
    | PortChanged of string
    | PassChanged of string
    | Connect
    | ConnectCompleted of OpenTTD * ServerWelcomeMessage
    | ConnectFailed

let connect model dispatch =
    let serverInfo = { Address = IPAddress.Parse (model.Host); Port = model.Port }
    let ottd = OpenTTD(serverInfo)
    ottd.TellConnect model.Pass
    ottd.OnWelcome.Publish
    |> Observable.subscribe (fun msg -> dispatch <| ConnectCompleted (ottd, msg))
    |> ignore

let init () =
    { Host = "127.0.0.1"
      Port = 3977
      Pass = "p7gvv" },
    Cmd.none
    
let update msg model =
    match msg with
    | HostChanged host -> {model with Host = host}, Cmd.none, NoOp
    | PortChanged port -> {model with Port = int port}, Cmd.none, NoOp
    | PassChanged pass -> {model with Pass = pass}, Cmd.none, NoOp 
    | Connect -> model, Cmd.ofSub (connect model), NoOp
    | ConnectCompleted (ottd, welcomeMsg) -> model, Cmd.none, Connected (ottd, welcomeMsg)
    | ConnectFailed -> model, Cmd.none, NoOp
    
let view model dispatch =
    Grid.create [
        Grid.children [
            StackPanel.create [
                StackPanel.verticalAlignment   VerticalAlignment.Center
                StackPanel.horizontalAlignment HorizontalAlignment.Center
                StackPanel.children [
                    TextBlock.create [
                        TextBlock.text "Connect to OpenTTD Admin"
                    ]
                    TextBox.create [
                        TextBox.text model.Host
                        TextBox.onTextChanged (fun text -> dispatch <| HostChanged text)
                    ]
                    TextBox.create [
                        TextBox.text <| string model.Port
                        TextBox.onTextChanged (fun text -> dispatch <| PortChanged text)
                    ]
                    TextBox.create [
                        TextBox.text model.Pass
                        TextBox.passwordChar '*'
                        TextBox.onTextChanged (fun text -> dispatch <| PassChanged text)
                    ]
                    Button.create [
                        Button.onClick (fun _ -> dispatch Connect)
                        Button.content "Connect"
                    ]
                ]
            ]
        ]
    ]