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


    public record OnProtocolMessage(Protocol Protocol);

    public record OnServerWelcomeMessage;
}