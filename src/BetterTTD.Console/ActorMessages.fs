module BetterTTD.Console.ActorMessagesModule

open System.Net
open BetterTTD.Console.PacketTransformers
open BetterTTD.FOAN.Network.Enums

type ReceiverMessage = Receive
    
type ConnectMessage = 
    { Address      : IPAddress
      Port         : int
      Password     : string }

type PollMessage =
    { UpdateType : AdminUpdateType
      Data       : uint64 }

type CoordinatorMessage =
    | Connect of ConnectMessage
    | ReceivedPacket of PacketMessage
    | Poll of PollMessage
