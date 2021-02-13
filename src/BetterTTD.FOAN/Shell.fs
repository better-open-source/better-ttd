namespace BetterTTD.FOAN

module Shell =
    
    open Elmish
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.DSL

    open Akka.FSharp
    open Akka.Actor
    
    open BetterTTD.FOAN.Actors.Actors.AdminCoordinator
    open BetterTTD.FOAN.Actors.Messages

    type ConnectionState =
        | Connected
        | Disconnected
    
    type State =
        { aboutState: About.State
          counterState: Counter.State
          loginState: Login.State
          connectionState: ConnectionState
          actorSystem : ActorSystem }

    type Msg =
        | AboutMsg of About.Msg
        | CounterMsg of Counter.Msg
        | LoginMsg of Login.Msg
        | UiMsg of UiMessage

    let init =
        let aboutState, _ = About.init
        let counterState  = Counter.init
        let loginState    = Login.init
        let system        = System.create "System" <| Configuration.load ()
        
        { aboutState      = aboutState
          counterState    = counterState
          loginState      = loginState
          connectionState = Disconnected
          actorSystem     = system },
        Cmd.none

    let update msg state =
        match msg with
        | AboutMsg aboutMsg ->
            let aboutState, _ = About.update aboutMsg state.aboutState
            { state with aboutState = aboutState }, Cmd.none
            
        | CounterMsg counterMsg ->
            let counterMsg = Counter.update counterMsg state.counterState
            { state with counterState = counterMsg }, Cmd.none
            
        | LoginMsg loginMsg ->
            match loginMsg with
            | Login.Connect ->
                let { Login.Host = host
                      Login.Pass = pass
                      Login.Port = port } = state.loginState
                select "user/adminCoordinator" state.actorSystem <! (Idle <| Connect(host, pass, port))
                state, Cmd.none
            | _ -> 
                let loginState = Login.update loginMsg state.loginState
                { state with loginState = loginState }, Cmd.none
                
        | UiMsg uiMsg ->
            match uiMsg with
            | ReceivedProtocol protocol ->
                printfn "%A" protocol
                state, Cmd.none
            | ReceivedWelcome  welcome  ->
                printfn "%A" welcome
                { state with connectionState = Connected }, Cmd.none
            | ConnectionClosed ->
                select "user/adminCoordinator" state.actorSystem <! PoisonPill.Instance
                { state with connectionState = Disconnected }, Cmd.none

    let connectedView state dispatch =
        DockPanel.create [
            DockPanel.children [
                TabControl.create [
                    TabControl.tabStripPlacement Dock.Left
                    TabControl.viewItems [
                        TabItem.create [
                            TabItem.header "Counter Sample"
                            TabItem.content (Counter.view state.counterState (CounterMsg >> dispatch))
                        ]
                        TabItem.create [
                            TabItem.header "About"
                            TabItem.content (About.view state.aboutState (AboutMsg >> dispatch))
                        ]
                    ]
                ]
            ]
        ]
        
    let disconnectedView state dispatch =
        DockPanel.create [
            DockPanel.children [
                (Login.view state.loginState (LoginMsg >> dispatch))
            ]
        ]
        
    let view (state: State) (dispatch) =
        match state.connectionState with
        | Connected    -> connectedView    state dispatch
        | Disconnected -> disconnectedView state dispatch
        
    let actorDispatch system =
        let sub dispatch =
            spawn system "adminCoordinator" <| adminCoordinator dispatch
            |> ignore
        Cmd.ofSub sub
    
    let subscription state =
        Cmd.batch [ Cmd.map UiMsg (actorDispatch state.actorSystem) ]
        
    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title     <- "BetterTTD - FOAN"
            base.Width     <- 800.0
            base.Height    <- 600.0
            base.MinWidth  <- 800.0
            base.MinHeight <- 600.0

            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
            |> Program.withSubscription subscription
            |> Program.run