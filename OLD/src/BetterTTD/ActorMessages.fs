module BetterTTD.ActorMessagesModule

open System.Net
open BetterTTD.PacketTransformers

type ReceiverMessage = Receive
    
type ConnectMessage = 
    { Address      : IPAddress
      Port         : int
      Password     : string }

type PollClientMessage =
    { ClientID : uint32 }

type CoordinatorMessage =
    | Connect of ConnectMessage
    | PacketReceived of PacketMessage
    | PollClient of PollClientMessage 
