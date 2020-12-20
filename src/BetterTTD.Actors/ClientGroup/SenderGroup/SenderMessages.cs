using BetterTTD.Domain.Enums;

namespace BetterTTD.Actors.ClientGroup.SenderGroup
{
    public class SendAdminUpdateFrequencyMessage
    {
        public AdminUpdateType Type { get; }
        public AdminUpdateFrequency Freq { get; }

        public SendAdminUpdateFrequencyMessage(
            AdminUpdateType type,
            AdminUpdateFrequency freq)
        {
            Type = type;
            Freq = freq;
        }
    }

    public class SendAdminJoinMessage
    {
        public string AdminPassword { get; }
        public string BotName { get; }
        public string BotVersion { get; }

        public SendAdminJoinMessage(
            string adminPassword,
            string botName,
            string botVersion)
        {
            AdminPassword = adminPassword;
            BotName = botName;
            BotVersion = botVersion;
        }
    }

    public class SendAdminPollMessage
    {
        public AdminUpdateType Type { get; }
        public long Data { get; }

        public SendAdminPollMessage(AdminUpdateType type, long data = 0)
        {
            Type = type;
            Data = data;
        }
    }
}