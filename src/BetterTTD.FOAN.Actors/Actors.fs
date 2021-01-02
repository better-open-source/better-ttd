namespace BetterTTD.FOAN.Actors


open Akka.FSharp
open Akka.Actor

open System
open System.Net.Sockets

open BetterTTD.FOAN.Actors.ActorStateMessages
open BetterTTD.FOAN.Actors.MessageTransform
open BetterTTD.FOAN.Network.PacketModule
open BetterTTD.FOAN.Network.Enums

module ActorsModule =

    let sender (socket : Socket) (mailbox : Actor<_>) =
        let rec loop() = actor {
            match! mailbox.Receive () with
            | Packet packet ->
                let { Buffer = buf; Position = pos } = prepareToSend packet
                let res = socket.Send (buf, pos, SocketFlags.None)
                printfn $"sent packet res {res}"
                ()
            return! loop()
        }
        loop ()
    
    let receiver (socket : Socket) (mailbox : Actor<_>) =
        let rec loop () = actor {
            match! mailbox.Receive () with
            | ReceiveMsg ->
                if socket.Connected then
                    let pac = createPacket
                    socket.Receive pac.Buffer |> ignore
                    let (x, pac) = readByte pac
                    let pacType = enum<AdminUpdateType>(Convert.ToInt32 x)
                    
                    printfn "received %A" pacType
                else
                    ()
            
            return! loop ()
        }
        loop ()
    
    let adminCoordinator (mailbox : Actor<_>) =
        
        let rec connected (receiver : IActorRef) (sender : IActorRef) (socket : Socket) = actor {
            return! connected receiver sender socket
        }
        
        let rec connecting (receiver : IActorRef) (sender : IActorRef) (socket : Socket) = actor {
            match! mailbox.Receive () with
            | Protocol protocol ->
                printfn "protocol received"
                ()
            | Welcome welcome ->
                printfn "welcome received"
                ()
            | _ -> failwithf "Invalid state operation occured"
            
            return! connecting receiver sender socket
        }
        
        let rec idle () = actor {
            match! mailbox.Receive() with
            | Connect(host, pass, port) ->
                let soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                soc.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true)
                soc.Connect(host, port)
                
                let senderRef = spawn mailbox "sender" (sender soc)
                let receiverRef = spawn mailbox "receiver" (receiver soc)
                let packet = AdminJoin { Password = pass; AdminName = "BetterTTD"; AdminVersion = "1.0" } |> msgToPacket
                
                mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromMilliseconds (0.),
                    TimeSpan.FromMilliseconds (1.),
                    receiverRef,
                    ReceiveMsg)
                
                senderRef <! Packet packet
                return! connecting receiverRef senderRef soc
            | _ -> failwithf "Invalid state operation occured"
                
            return! idle ()
        }
        
        idle ()