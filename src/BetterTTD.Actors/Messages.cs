using System.Collections.Generic;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterTTD.Actors
{
    public record ReceivedBufMessage(Packet Packet);
    public record ReceiveBufMessage;

    public record SendAdminUpdateFrequencyMessage(
        AdminUpdateType Type, 
        AdminUpdateFrequency Freq);
    public record SendAdminJoinMessage(
        string AdminPassword, 
        string BotName, 
        string BotVersion);
    public record SendAdminPollMessage(AdminUpdateType Type, long Data = 0);
    
    public record AdminConnectMessage(
        string Host, 
        int Port, 
        string AdminPassword);

    public record SetDefaultUpdateFrequencyMessage(Protocol Protocol);

    public record PollAllMessage(Protocol Protocol);


    public record OnServerCmdNamesMessage(Dictionary<int, string> CmdNames);
    public record OnServerWelcomeMessage(Game Game);
    public record OnProtocolMessage(Protocol Protocol);
    public record OnServerConsoleMessage(string Origin, string Message);
    public record OnServerClientInfoMessage(Client Client);
    public record OnServerChatMessage(
        NetworkAction Action, 
        DestType Dest, 
        long ClientId, 
        string Message,
        long Data);
    public record OnServerClientUpdateMessage(
        long ClientId,
        string Name,
        int CompanyId);
    public record OnServerClientQuitMessage(long ClientId);
    public record OnServerClientErrorMessage(long ClientId, NetworkErrorCode ErrorCode);
    public record OnServerCompanyStatsMessage(
        int CompanyId,
        Dictionary<VehicleType, int> Vehicles,
        Dictionary<VehicleType, int> Stations);
    public record OnServerCompanyRemoveMessage(int CompanyId, AdminCompanyRemoveReason RemoveReason);
}