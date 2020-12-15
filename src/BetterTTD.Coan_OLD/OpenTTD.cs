using System;
using System.Numerics;
using BetterTTD.Coan_OLD.Networks;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Coan_OLD
{
    public class OpenTTD
    {
        public Game Game { get; } = new();
        public Pool Pool { get; } = new();
        public Network Network { get; }

        public string BotName { get; set; }
        public string BotVersion { get; set; }

        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 3977;
        public string Password { get; set; }

        public OpenTTD()
        {
            Network = new(this);
        }

        public bool Connect(string host, int port, string password)
        {
            HostName = host;
            Port = port;
            Password = password;

            return Connect();
        }

        public bool Connect()
        {
            if (!Network.Connect(HostName, Port))
            {
                return false;
            }
            
            Network.Receive();
            return true;
        }

        protected void RegisterUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            //Network.SendAdminUpdateFrequency(type, freq);
        }

        public void PollAll()
        {
            PollCmdNames();
            PollDate();
            PollClientInfos();
            PollCompanyInfos();
            PollCompanyStats();
            PollCompanyEconomy();
        }

        public void PollDate()
        {
            Network.PollDate();
        }

        public void PollClientInfos()
        {
            Network.PollClientInfos();
        }

        public void PollClientInfo(long clientId)
        {
            Network.PollClientInfo(clientId);
        }

        public void PollCompanyInfos()
        {
            Network.PollCompanyInfos();
        }

        public void PollCompanyInfo(int companyId)
        {
            Network.PollCompanyInfo(companyId);
        }

        public void PollCompanyStats()
        {
            Network.PollCompanyStats();
        }

        public void PollCompanyEconomy()
        {
            Network.PollCompanyEconomy();
        }

        public void PollCmdNames()
        {
            Network.PollCmdNames();
        }

        public void ServerMessagePublic(string msg)
        {
            Network.ServerMessagePublic(msg);
        }

        public void ServerMessagePrivate(long client, string msg)
        {
            Network.ServerMessagePrivate(client, msg);
        }

        public void SendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            //Network.SendAdminUpdateFrequency(type, freq);
        }

        public void SendAdminRcon(string command)
        {
            Network.SendAdminRcon(command);
        }

        public void SendAdminQuit()
        {
            Network.SendAdminQuit();
        }

        public void SendAdminChat(NetworkAction action, DestType type, long dest, string message, long data)
        {
            //Network.SendAdminChat(action, type, dest, message, data);
        }

        public void SendAdminGameScript(string json)
        {
            Network.SendAdminGameScript(json);
        }

        public void SendAdminPing(long d1)
        {
            Network.SendAdminPing(d1);
        }

        public void ChatPublic(string msg)
        {
            Network.ChatPublic(msg);
        }

        public void ChatPrivate(long client, string msg)
        {
            Network.ChatPrivate(client, msg);
        }

        public void OnServerFull()
        {
            Console.WriteLine($"{nameof(OnServerFull)}");
        }

        public void OnServerBanned()
        {
            Console.WriteLine($"{nameof(OnServerBanned)}");
        }

        public void OnServerError(NetworkErrorCode error)
        {
            Console.WriteLine($"{nameof(OnServerError)}");
        }

        public void OnServerWelcome(Game game)
        {
            Console.WriteLine($"{nameof(OnServerWelcome)}");
        }

        public void OnServerDate(GameDate date)
        {
            Console.WriteLine($"{nameof(OnServerDate)}");
        }

        public void OnClientJoin(Client client)
        {
            Console.WriteLine($"{nameof(OnClientJoin)}");
        }

        public void OnClientInfo(Client client)
        {
            Console.WriteLine($"{nameof(OnClientInfo)}");
        }

        public void OnClientUpdate(Client client)
        {
            Console.WriteLine($"{nameof(OnClientUpdate)}");
        }

        public void OnClientQuit(Client client)
        {
            Console.WriteLine($"{nameof(OnClientQuit)}");
        }

        public void OnClientError(Client client, NetworkErrorCode error)
        {
            Console.WriteLine($"{nameof(OnClientError)}");
        }

        public void OnCompanyNew(Company company)
        {
            Console.WriteLine($"{nameof(OnCompanyNew)}");
        }

        public void OnCompanyInfo(Company company)
        {
            Console.WriteLine($"{nameof(OnCompanyInfo)}");
        }

        public void OnCompanyUpdate(Company company)
        {
            Console.WriteLine($"{nameof(OnCompanyUpdate)}");
        }

        public void OnCompanyStats(Company company)
        {
            Console.WriteLine($"{nameof(OnCompanyStats)}");
        }

        public void OnCompanyEconomy(Company company)
        {
            Console.WriteLine($"{nameof(OnCompanyEconomy)}");
        }

        public void OnCompanyRemove(Company company, AdminCompanyRemoveReason crr)
        {
            Console.WriteLine($"{nameof(OnCompanyRemove)}");
        }

        public void OnRcon(RconBuffer rconBuffer)
        {
            Console.WriteLine($"{nameof(OnRcon)}");
        }

        public void OnChat(NetworkAction action, DestType destType, Client client, string message, BigInteger data)
        {
            Console.WriteLine($"{nameof(OnChat)}");
        }

        public void OnNewGame()
        {
            Console.WriteLine($"{nameof(OnNewGame)}");
        }

        public void OnShutdown()
        {
            Console.WriteLine($"{nameof(OnShutdown)}");
        }

        public void OnProtocol(Protocol protocol)
        {
            Console.WriteLine($"{nameof(OnProtocol)}");
        }

        public void OnConsole(string origin, string message)
        {
            Console.WriteLine($"{nameof(OnConsole)}");
        }

        public void OnCmdLogging(Client client, Company company, DoCommandName command, long p1, long p2, long tile,
            string text, long frame)
        {
            Console.WriteLine($"{nameof(OnCmdLogging)}");
        }

        public void OnGameScript(string json)
        {
            Console.WriteLine($"{nameof(OnGameScript)}");
        }

        public void OnPong(long payload)
        {
            Console.WriteLine($"{nameof(OnPong)}");
        }

        public void OnPause(PauseMode pm, bool paused)
        {
            Console.WriteLine($"{nameof(OnPause)}");
        }
    }
}