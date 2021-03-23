module BetterTTD.Main

open Avalonia.FuncUI.Types
open Avalonia.Platform
open Elmish
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.Components.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Builder


type Msg =
    | None

type Model =
    { Id : int }

let init =
    { Id = 0 }, Cmd.none
    
let update (msg : Msg) (model : Model) =
    match msg with
    | None -> model, Cmd.none

let aboutPageContent = 
    DockPanel.create [ 
        DockPanel.children [
            TextBox.create [ TextBox.text "About" ]
        ]
    ]
    
let tabs : IView list = [
    TabItem.create [
        TabItem.header "Shell"
        TabItem.content (ViewBuilder.Create<Shell.Host>([]))
    ]
    TabItem.create [
        TabItem.header "About"
        TabItem.content aboutPageContent
    ]
]

let view (model : Model) dispatch =
    TabControl.create [
        TabControl.viewItems tabs
    ]

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title      <- "BetterTTD - FOAN"
        base.Width      <- 800.0
        base.Height     <- 600.0
        base.MinWidth   <- 800.0
        base.MinHeight  <- 600.0
        base.Icon       <- null
        base.TransparencyLevelHint <- WindowTransparencyLevel.None
        base.SystemDecorations <- SystemDecorations.Full
        base.ExtendClientAreaChromeHints <- ExtendClientAreaChromeHints.PreferSystemChrome
        base.ExtendClientAreaToDecorationsHint <- true
        base.ExtendClientAreaTitleBarHeightHint <- -1.

        Elmish.Program.mkProgram (fun () -> init) update view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.run