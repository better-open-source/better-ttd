namespace BetterTTD.FOAN.Actors

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule

module ActorStateMessages =
     
    type AdminServerProtocolMessage = {
        Version : byte
        UpdateSettings : Map<AdminUpdateType, AdminUpdateFrequency>
    }
    
    type AdminServerWelcomeMessage = {
        ServerName      : string option
        NetworkRevision : string option
        IsDedicated     : bool
        MapName         : string option
        MapSeed         : uint32
        Landscape       : Landscape
        CurrentDate     : string option
        MapWidth        : uint16
        MapHeight       : uint16
    }

    type AdminJoinMessage =
        { Password     : string
          AdminName    : string
          AdminVersion : string }
        
    type AdminMessage =
        | AdminJoin of AdminJoinMessage

    type ReceiverMessage = ReceiveMsg
    
    type SenderMessage = Packet of Packet
               
    type AdminCoordinatorMessage =
        | Connect of host : string *
                     pass : string *
                     port : int
        | Protocol of AdminServerProtocolMessage
        | Welcome of AdminServerWelcomeMessage
    