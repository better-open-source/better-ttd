module BetterTTD.CoordinatorModule

open System
open System.Net
open System.Net.Sockets
open Akka.FSharp
open BetterTTD.ActorMessagesModule
open BetterTTD.MessageTransformers
open BetterTTD.PacketTransformers
open BetterTTD.ReceiverModule
open BetterTTD.SenderModule
open BetterTTD.ServicesModule
open BetterTTD.Network.Enums

let defaultUpdateFrequencies =
    [ { UpdateType = AdminUpdateType.ADMIN_UPDATE_CHAT
        Frequency  = AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC }
      { UpdateType = AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO
        Frequency  = AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC } ]
    |> List.map AdminUpdateFreq
    
let connectToStream (ipAddress : IPAddress) (port : int) =
    let tcpClient = new TcpClient ()
    tcpClient.Connect (ipAddress, port)
    tcpClient.GetStream ()
    
let scheduleMailbox (mailbox : Actor<_>) ref interval msg =
    mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
        TimeSpan.FromMilliseconds (0.),
        TimeSpan.FromMilliseconds (interval),
        ref,
        msg)
    

let coordinator (subscriber : IServerSubscriber) (mailbox : Actor<CoordinatorMessage>) =
    let schedule = scheduleMailbox mailbox
    
    let rec connected sender receiver =
        actor {
            match! mailbox.Receive () with
            | PacketReceived pac -> subscriber.OnPacketReceived pac
            | PollClient { ClientID = clientId } ->
                sender <! AdminPoll { UpdateType = AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO
                                      Data       = clientId }
            | _ -> failwith "INVALID CONNECTED STATE CAPTURED"
            return! connected sender receiver
        }
        
    and connecting sender receiver =
        actor {
            match! mailbox.Receive () with
            | PacketReceived pac ->
                subscriber.OnPacketReceived pac
                match pac with
                | ServerProtocol _ -> defaultUpdateFrequencies |> List.iter (fun msg -> sender <! msg)
                | ServerWelcome _ -> return! connected sender receiver
                | _ -> failwithf "INVALID CONNECTING STATE CAPTURED FOR PACKET: %A" pac
            | _ -> failwith "INVALID CONNECTING STATE CAPTURED"
            return! connecting sender receiver
        }
        
    and idle () =
        actor {
            match! mailbox.Receive () with
            | Connect { Address = addr; Port = port; Password = pass} ->
                let networkStream = connectToStream addr port
                            
                let receiverRef = spawn mailbox "receiver" <| (receiver networkStream)
                schedule receiverRef 100.0 Receive
                
                let senderRef = spawn mailbox "sender" <| (sender networkStream)
                senderRef <! AdminJoin {Password = pass; AdminName = "BetterTTD"; AdminVersion = "1.0"}
                
                return! connecting senderRef receiverRef
            | _ -> failwith "INVALID IDLE STATE CAPTURED"
        }

    idle ()
       