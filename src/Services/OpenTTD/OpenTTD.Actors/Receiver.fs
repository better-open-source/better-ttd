namespace OpenTTD.Actors

module Receiver =

    open System
    open System.IO
    
    open Akka.FSharp
    open FSharpx.Collections
    
    open OpenTTD.Actors.Messages
    open OpenTTD.Network.Packet
    open OpenTTD.Network.PacketTransformers

    let private read (stream : Stream) (size : int) =
        let buf = Array.zeroCreate<byte> size
        
        let rec tRead (tStream : Stream) (tSize : int) =
            if tSize < size then
                let res = tStream.Read (buf, tSize, size - tSize)
                tRead tStream (tSize + res)
            else tSize
            
        tRead stream 0 |> ignore
        buf

    let private createPacket (sizeBuf : byte array) (content : byte array) =
        let buf = Array.zeroCreate<byte> (2 + content.Length)
        buf.[0] <- sizeBuf.[0]
        buf.[1] <- sizeBuf.[1]
        for i in 0 .. (content.Length - 1) do
            buf.[i + 2] <- content.[i]
        { createPacket with Buffer = buf }

    let private waitForPacket (stream : Stream) =
        let sizeBuf = read stream 2
        let size = BitConverter.ToUInt16 (sizeBuf, 0)
        let content = read stream (int size - 2)
        createPacket sizeBuf content

    let actor (stream : Stream) (mailbox : Actor<ReceiverMessage>) =
        let rec loop () =
            actor {
                match! mailbox.Receive () with
                | _ ->
                    let pac = waitForPacket stream
                    let msg = packetToMsg pac
                    mailbox.Context.Parent <! PacketReceived msg
                return! loop ()
            }
            
        loop ()
