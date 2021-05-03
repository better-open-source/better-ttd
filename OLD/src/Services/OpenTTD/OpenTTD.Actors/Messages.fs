namespace OpenTTD.Actors

module Messages =

    open System.Net
    
    open OpenTTD.Network.PacketTransformers
    
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
