module BetterTTD.OpenTTDModule

open System
open System.Net
open Akka.FSharp
open BetterTTD.CoordinatorModule
open BetterTTD.ActorMessagesModule
open BetterTTD.PacketTransformers
open BetterTTD.ServicesModule

type ServerInfo =
    { Address : IPAddress
      Port    : int }

type OpenTTD (serverInfo : ServerInfo) =
    let onUpdate = Event<PacketMessage>()
    let onProtocol = Event<ServerProtocolMessage>()
    let onWelcome = Event<ServerWelcomeMessage>()
    let onChat = Event<ServerChatMessage>()
    let onClientJoin = Event<ServerClientJoinMessage>()
    let onClientInfo = Event<ServerClientInfoMessage>()
    let onClientUpdate = Event<ServerClientUpdateMessage>()
    let onClientError = Event<ServerClientErrorMessage>()
    let onClientQuit = Event<ServerClientQuitMessage>()

    let subscriber =
        { new IServerSubscriber with
            member __.OnPacketReceived pac =
                onUpdate.Trigger pac
                match pac with
                | ServerProtocol     protocol -> onProtocol.Trigger     protocol
                | ServerWelcome      welcome  -> onWelcome.Trigger      welcome
                | ServerChat         chat     -> onChat.Trigger         chat
                | ServerClientJoin   client   -> onClientJoin.Trigger   client
                | ServerClientInfo   client   -> onClientInfo.Trigger   client
                | ServerClientUpdate client   -> onClientUpdate.Trigger client
                | ServerClientError  client   -> onClientError.Trigger  client
                | ServerClientQuit   client   -> onClientQuit.Trigger   client }
    
    let notifier =
        let system = System.create "tempServName" <| Configuration.load ()
        let coordinatorRef = spawn system "coordinator" <| coordinator subscriber
        { new IServerNotifier with
            member __.SendConnect msg =
                coordinatorRef <! Connect msg
            member __.SendPollClient msg =
                coordinatorRef <! PollClient msg }
    
    member this.OnUpdate = onUpdate
    member this.OnProtocol = onProtocol
    member this.OnWelcome = onWelcome
    member this.OnChat = onChat
    member this.OnClientJoin = onClientJoin
    member this.OnClientInfo = onClientInfo
    member this.OnClientUpdate = onClientUpdate
    member this.OnClientError = onClientError
    member this.OnClientQuit = onClientQuit
    
    member this.TellConnect (pass : string) =
        notifier.SendConnect
            { Address  = serverInfo.Address
              Port     = serverInfo.Port
              Password = pass }
        
    member this.AskPollClient (clientId : uint32) =
        notifier.SendPollClient
            { ClientID = clientId }
