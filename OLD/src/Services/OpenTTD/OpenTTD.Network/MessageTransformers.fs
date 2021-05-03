namespace OpenTTD.Network

module MessageTransformers =

    open OpenTTD.Network.Enums
    open OpenTTD.Network.Packet

    type AdminJoinMessage =
        { Password     : string
          AdminName    : string
          AdminVersion : string }

    type AdminUpdateFreqMessage =
        { UpdateType   : AdminUpdateType
          Frequency    : AdminUpdateFrequency }

    type AdminPollMessage =
        { UpdateType : AdminUpdateType
          Data       : uint32 }

    type AdminMessage =
        | AdminJoin of AdminJoinMessage
        | AdminUpdateFreq of AdminUpdateFreqMessage
        | AdminPoll of AdminPollMessage


    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_JOIN
            |> writeString pass
            |> writeString name
            |> writeString version
        | AdminUpdateFreq { UpdateType = update; Frequency = freq } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY
            |> writeU16 (uint16 update)
            |> writeU16 (uint16 freq)
        | AdminPoll { UpdateType = update; Data = data } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_POLL
            |> writeByte (byte update)
            |> writeU32 data
           