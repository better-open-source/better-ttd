namespace BetterTTD.FOAN.Actors.Actors

module AdminCoordinator =

    open Akka.FSharp
    open Akka.Actor

    open System
    open System.Net.Sockets

    open BetterTTD.FOAN.Actors.Messages
    open BetterTTD.FOAN.Actors.Transformers.MessageTransformer
    open BetterTTD.FOAN.Actors.Receiver
    open BetterTTD.FOAN.Actors.Sender
    
    let private createSocket (host : string) (port : int) = 
        let soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        soc.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true)
        soc.Connect(host, port)
        soc

    let adminCoordinator dispatch (mailbox : Actor<_>) =
        
        let rec erroredOut errorMsg =
            actor {
                match errorMsg with
                | SocketConnectionClosed ->
                    printfn "SocketConnectionClosed"
                    dispatch ConnectionClosed
                    return! idle ()
                | UnhandledNetworkError ->
                    printfn "UnhandledNetworkError"
                    dispatch ConnectionClosed
                    return! idle ()
            }
        
        and connected (receiver : IActorRef) (sender : IActorRef) (socket : Socket) =
            actor {
                match! mailbox.Receive () with
                | ErroredOut error -> return! erroredOut error
                | _ -> return! connected receiver sender socket
            }
            
        and connecting (receiver : IActorRef) (sender : IActorRef) (socket : Socket) =
            actor {
                let matchConnecting = function
                | Protocol protocol ->
                    dispatch <| ReceivedProtocol protocol
                    connecting receiver sender socket
                | Welcome welcome ->
                    dispatch <| ReceivedWelcome welcome
                    connected receiver sender socket
                
                match! mailbox.Receive () with
                | Connecting msg -> return! matchConnecting msg
                | ErroredOut error -> return! erroredOut error
                | _ -> failwithf "Invalid state operation occured"
            }
            
        and idle () =
            actor {
                let matchIdle = function
                | Connect(host, pass, port) ->
                    let soc = createSocket host port
                    let senderRef = spawn mailbox "sender" (sender soc)
                    let receiverRef = spawn mailbox "receiver" (receiver soc)
                    let msg = AdminJoin { Password = pass; AdminName = "BetterTTD"; AdminVersion = "1.0" }
                    let packet = msgToPacket msg
                    
                    mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromMilliseconds (0.),
                        TimeSpan.FromMilliseconds (1.),
                        receiverRef,
                        ReceiveMsg)
                    
                    senderRef <! Packet packet
                    connecting receiverRef senderRef soc
                    
                match! mailbox.Receive () with
                | Idle msg -> return! matchIdle msg
                | _ -> failwithf "Invalid state operation occured"
                    
                return! idle ()
            }
        
        idle ()