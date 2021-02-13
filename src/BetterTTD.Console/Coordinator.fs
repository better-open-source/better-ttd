module BetterTTD.Console.CoordinatorModule

open System
open System.Net
open System.Net.Sockets
open Akka.FSharp
open BetterTTD.Console.ActorMessagesModule
open BetterTTD.Console.MessageTransformers
open BetterTTD.Console.PacketTransformers
open BetterTTD.Console.ReceiverModule
open BetterTTD.Console.SenderModule
open BetterTTD.FOAN.Network.Enums

let defaultUpdateFrequencies =
    [ { UpdateType = AdminUpdateType.ADMIN_UPDATE_CHAT
        Frequency  = AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC }
      { UpdateType = AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO
        Frequency  = AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC } ]
    
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

let coordinator (mailbox : Actor<CoordinatorMessage>) =
    let schedule = scheduleMailbox mailbox
    
    let rec connected sender receiver =
        actor {
            match! mailbox.Receive () with
            | ReceivedPacket (ServerChat chat)            -> printfn "chat %A" chat
            | ReceivedPacket (ServerClientJoin client)    -> printfn "join %A" client
            | ReceivedPacket (ServerClientInfo client)    -> printfn "info %A" client
            | ReceivedPacket (ServerClientUpdate client)  -> printfn "update%A" client
            | ReceivedPacket (ServerClientError client)   -> printfn "error %A" client
            | ReceivedPacket (ServerClientQuit client)    -> printfn "quit %A" client
            | _ -> failwith "INVALID CONNECTED STATE CAPTURED"
            return! connected sender receiver
        }
        
    and connecting sender receiver =
        actor {
            match! mailbox.Receive () with
            | ReceivedPacket (ServerProtocol protocol) ->
                defaultUpdateFrequencies
                |> List.map AdminUpdateFreq
                |> List.iter (fun msg -> sender <! msg)
                printfn "%A" protocol
            | ReceivedPacket (ServerWelcome welcome) ->
                printfn "%A" welcome
                return! connected sender receiver
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
       