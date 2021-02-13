namespace BetterTTD.FOAN.Actors

module Sender =

    open System.Net.Sockets

    open Akka.FSharp

    open BetterTTD.FOAN.Actors.Messages
    open BetterTTD.FOAN.Network.PacketModule

    let sender (socket : Socket) (mailbox : Actor<_>) =
        let rec loop() =
            actor {
                match! mailbox.Receive () with
                | Packet packet ->
                    let { Buffer = buf; Size = size; } = prepareToSend packet
                    socket.Send (buf, int size, SocketFlags.None) |> ignore
                    ()
                return! loop()
            }
            
        loop ()
    