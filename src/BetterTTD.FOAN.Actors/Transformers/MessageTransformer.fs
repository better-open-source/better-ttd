namespace BetterTTD.FOAN.Actors.Transformers

module MessageTransformer =
    
    open BetterTTD.FOAN.Network.Enums
    open BetterTTD.FOAN.Actors.Messages
    open BetterTTD.FOAN.Network.PacketModule

    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_JOIN
            |> writeString pass
            |> writeString name
            |> writeString version