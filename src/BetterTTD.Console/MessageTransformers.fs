module BetterTTD.Console.MessageTransformers

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule

type AdminJoinMessage =
    { Password     : string
      AdminName    : string
      AdminVersion : string }

type AdminUpdateFreqMessage =
    { UpdateType   : AdminUpdateType
      Frequency    : AdminUpdateFrequency }

type AdminMessage =
    | AdminJoin of AdminJoinMessage
    | AdminUpdateFreq of AdminUpdateFreqMessage


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
       