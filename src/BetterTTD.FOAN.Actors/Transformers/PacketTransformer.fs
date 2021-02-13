namespace BetterTTD.FOAN.Actors.Transformers

module PacketTransformer =
    
    open System

    open FSharpx.Collections
    
    open BetterTTD.FOAN.Network.Enums
    open BetterTTD.FOAN.Actors.Messages
    open BetterTTD.FOAN.Network.PacketModule
    
    let readServerProtocol packet =
        let (version, packet) = readByte packet
        
        let rec readFreq (dict : Map<AdminUpdateType, AdminUpdateFrequency []>) pac =
            let (next, pac) = readBool pac
            if (next) then
                let (updIdx, pac) = readU16 pac
                let (freqIdx, pac) = readU16 pac
                let upd = enum<AdminUpdateType>(int updIdx)
                let newFrequencies =
                    Enum.GetValues<AdminUpdateFrequency>()
                    |> Array.filter (fun freq -> (int freqIdx &&& (int freq)) <> 0)
                    |> Array.map (fun freq -> (upd, freq))
                    |> Array.groupBy fst
                    |> Array.map (fun (key, items) ->
                        key, items |> Array.map snd |> Array.ofSeq)
                    |> Map.ofSeq
                let newDict = Map.union dict newFrequencies
                readFreq newDict pac
            else
                dict, pac
                
        let (dict, _) = readFreq Map.empty packet
        AdminServerProtocol
            { Version = version
              UpdateSettings = dict }
    
    let readServerWelcome packet =
        let (serverName, pac) = readString packet
        let (networkRevision, pac) = readString pac
        let (isDedicated, pac) = readBool pac
        let (mapName, pac) = readString pac
        let (mapSeed, pac) = readU32 pac
        let (x, pac) = readByte pac
        let landscape = enum<Landscape>(int x)
        let (currentDate, pac) = readU32 pac
        let (mapWidth, pac) = readU16 pac
        let (mapHeight, _) = readU16 pac
        AdminServerWelcome
            { ServerName = serverName
              NetworkRevision = networkRevision
              IsDedicated = isDedicated
              MapName = mapName
              MapSeed = mapSeed
              Landscape = landscape
              CurrentDate = currentDate
              MapWidth = int mapWidth
              MapHeight = int mapHeight }
    
    let packetToMsg packet =
        let (x, pac) = readByte packet
        match enum<PacketType>(int x) with
        | PacketType.ADMIN_PACKET_SERVER_PROTOCOL -> readServerProtocol pac
        | PacketType.ADMIN_PACKET_SERVER_WELCOME  -> readServerWelcome pac
        | _ -> failwithf "PACKET TRANSFORMER ERROR."