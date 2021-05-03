namespace OpenTTD.Actors

module Sender =

    open System.IO
    
    open Akka.FSharp
    
    open OpenTTD.Network.MessageTransformers
    open OpenTTD.Network.Packet
     
    let actor (stream : Stream) (mailbox : Actor<AdminMessage>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive ()
                let { Buffer = buf; Size = size; } = msg |> msgToPacket |> prepareToSend
                stream.Write (buf, 0, int size)
                return! loop ()
            }
            
        loop ()
        