module BetterTTD.Console.SenderModule

open System.IO
open Akka.FSharp
open BetterTTD.Console.MessageTransformers
open BetterTTD.FOAN.Network.PacketModule
 
let sender (stream : Stream) (mailbox : Actor<AdminMessage>) =
    let rec loop () =
        actor {
            let! msg = mailbox.Receive ()
            let { Buffer = buf; Size = size; } = msg |> msgToPacket |> prepareToSend
            stream.Write (buf, 0, int size)
            return! loop ()
        }
        
    loop ()
    