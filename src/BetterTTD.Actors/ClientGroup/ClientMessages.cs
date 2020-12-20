using BetterTTD.Network;

namespace BetterTTD.Actors.ClientGroup
{
    public class AdminConnectMessage
    {
        public string Host { get; }
        public int Port { get; }
        public string AdminPassword { get; }

        public AdminConnectMessage(
            string host,
            int port,
            string adminPassword)
        {
            Host = host;
            Port = port;
            AdminPassword = adminPassword;
        }
    }

    public class OnAdminConnectMessage { }
    
    public class SetDefaultUpdateFrequencyMessage
    {
        public Protocol Protocol { get; }

        public SetDefaultUpdateFrequencyMessage(Protocol protocol)
        {
            Protocol = protocol;
        }
    }

    public class PollAllMessage
    {
        public Protocol Protocol { get; }

        public PollAllMessage(Protocol protocol)
        {
            Protocol = protocol;
        }
    }
}