using System;
using System.Net.Sockets;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Coan_OLD.Networks
{
    public class Network
    {
        private readonly NetworkClient _networkClient;
        
        public Socket Socket { get; private set; }
        public OpenTTD OpenTTD { get; }
        public Protocol Protocol { get; } = new();
        public NetworkThreadBase InputThread { get; } = new NetworkInputThread();
        public NetworkThreadBase OutputThread { get; } = new NetworkOutputThread();

        public Network(OpenTTD ottd)
        {
            OpenTTD = ottd;
            _networkClient = new NetworkClient(this);
        }

        public bool Connect(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(OpenTTD.Password))
            {
                return false;
            }

            try
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                Socket.Connect(host, port);

                _networkClient.SendAdminJoin();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool IsConnected()
        {
            var part1 = Socket.Poll(1000, SelectMode.SelectRead);
            var part2 = Socket.Available == 0;
            return !part1 || !part2;
        }

        public void Disconnect()
        {
            try
            {
                Socket?.Disconnect(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Receive()
        {
            _networkClient.Run();
        }

        public void ServerMessagePublic(string msg)
        {
            _networkClient.SendAdminChat(NetworkAction.NETWORK_ACTION_SERVER_MESSAGE, DestType.DESTTYPE_BROADCAST, 0, msg, 0);
        }

        public void ServerMessagePrivate(long client, string msg)
        {
            _networkClient.SendAdminChat(NetworkAction.NETWORK_ACTION_SERVER_MESSAGE, DestType.DESTTYPE_BROADCAST, client, msg, 0);
        }

        public void ChatPublic(string msg)
        {
            _networkClient.SendAdminChat(NetworkAction.NETWORK_ACTION_CHAT, DestType.DESTTYPE_BROADCAST, 0, msg, 0);
        }

        public void ChatPrivate(long client, string msg)
        {
            _networkClient.SendAdminChat(NetworkAction.NETWORK_ACTION_CHAT_CLIENT, DestType.DESTTYPE_CLIENT, client, msg, 0);
        }

        public void ChatTeam(int company, string msg)
        {
            _networkClient.SendAdminChat(NetworkAction.NETWORK_ACTION_SERVER_MESSAGE, DestType.DESTTYPE_TEAM, company, msg, 0);
        }

        public void SendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            _networkClient.SendAdminUpdateFrequency(type, freq);
        }

        public void SendAdminRcon(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
                _networkClient.SendAdminRcon(command);
        }

        public void SendAdminQuit()
        {
            _networkClient.SendAdminQuit();
        }

        public void SendAdminChat(NetworkAction action, DestType type, long dest, string message, long data)
        {
            _networkClient.SendAdminChat(action, type, dest, message, data);
        }

        public void SendAdminGameScript(string json)
        {
            _networkClient.SendAdminGameScript(json);
        }

        public void SendAdminPing(long d1)
        {
            _networkClient.SendAdminPing(d1);
        }

        public void PollDate()
        {
            _networkClient.PollDate();
        }

        public void PollCompanyStats()
        {
            _networkClient.PollCompanyStats();
        }

        public void PollCompanyInfos()
        {
            _networkClient.PollCompanyInfos();
        }

        public void PollCompanyInfo(int companyId)
        {
            _networkClient.PollCompanyInfo(companyId);
        }

        public void PollCompanyEconomy()
        {
            _networkClient.PollCompanyEconomy();
        }

        public void PollClientInfos()
        {
            _networkClient.PollClientInfos();
        }

        public void PollClientInfo(long clientId)
        {
            _networkClient.PollClientInfo(clientId);
        }

        public void PollCmdNames()
        {
            _networkClient.PollCmdNames();
        }
    }
}