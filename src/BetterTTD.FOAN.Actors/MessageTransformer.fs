namespace BetterTTD.FOAN.Actors

open System

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Actors.Messages
open BetterTTD.FOAN.Network.PacketModule

module MessageTransformer =
        
    let private (~%) (pacType : PacketType) =
        Convert.ToByte pacType
    
    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_JOIN
            |> writeString pass
            |> writeString name
            |> writeString version
            
    let readServerProtocol (packet : Packet) =
        let (version, packet) = readByte packet
        let rec readFreq (dict : Map<AdminUpdateType, AdminUpdateFrequency>) pac =
            let (next, pac) = readBool pac
            if (next) then
                let (x, pac) = readU16 pac
                let updateType = enum<AdminUpdateType>(int x)
                let (x, pac) = readU16 pac
                let freq = enum<AdminUpdateFrequency>(int x)
                let newDict = dict.Add(updateType, freq)
                readFreq newDict pac
            else
                dict, pac
            
        let dict, _ = readFreq Map.empty packet
        AdminServerProtocol { Version = version; UpdateSettings = dict }
            
    let packetToMsg packet =
        let (x, pac) = readByte packet
        match enum<PacketType>(int x) with
        | PacketType.ADMIN_PACKET_SERVER_PROTOCOL -> readServerProtocol pac
        | _ -> failwithf "PACKET TRANSFORMER ERROR."