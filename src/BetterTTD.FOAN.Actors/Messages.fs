namespace BetterTTD.FOAN.Actors

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule

module MessagesTypes =

    type AdminJoinMessage =
        { Password     : string
          AdminName    : string
          AdminVersion : string }
        
    type AdminServerProtocolMessage =
        { Version        : byte
          UpdateSettings : Map<AdminUpdateType, AdminUpdateFrequency []> }

    type AdminServerWelcomeMessage =
        { ServerName      : string 
          NetworkRevision : string 
          IsDedicated     : bool 
          MapName         : string 
          MapSeed         : uint32 
          Landscape       : Landscape 
          CurrentDate     : uint32 
          MapWidth        : int 
          MapHeight       : int }

module Messages =
    
    open MessagesTypes

    type UiMessage =
        | ReceivedProtocol of AdminServerProtocolMessage
        | ReceivedWelcome  of AdminServerWelcomeMessage
        | ConnectionClosed
        
    type PacketMessage =
        | AdminServerProtocol of AdminServerProtocolMessage
        | AdminServerWelcome  of AdminServerWelcomeMessage
        
    type AdminMessage =
        | AdminJoin of AdminJoinMessage
    
    type ReceiverMessage = ReceiveMsg
    
    type SenderMessage = Packet of Packet
               
    type IdleMessage =
        | Connect of host : string *
                     pass : string *
                     port : int
                     
    type ConnectingMessage =
        | Protocol of AdminServerProtocolMessage
        | Welcome  of AdminServerWelcomeMessage
        
    type ConnectedMessage =
        | CaseOne
        | CaseTwo
    
    type ErroredOutMessage =
        | SocketConnectionClosed
        | UnhandledNetworkError
    
    type AdminCoordinatorMessage =
        | Idle       of IdleMessage
        | Connecting of ConnectingMessage
        | Connected  of ConnectedMessage
        | ErroredOut of ErroredOutMessage
    