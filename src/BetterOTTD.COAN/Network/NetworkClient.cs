using System;
using System.Net.Sockets;
using System.Threading;
using BetterOTTD.COAN.Common;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterOTTD.COAN.Network
{
    public class NetworkClient
    {
        private readonly Protocol _protocol;
        private Socket _socket;
        private readonly Thread _mThread;

        public string botName = "Bot Name";
        public string botVersion = "BOT VERSION";

        public string adminHost = "";
        public string adminPassword = "";
        public int adminPort = 3978;

        #region Delegates
        
        public delegate void onChat(string origin, string message);
        public delegate void onClientInfo(Client client);
        public delegate void onProtocol(Protocol protocol);
        public delegate void onWelcome();
        
        #endregion

        #region Events
        
        public event onChat OnChat;
        public event onClientInfo OnClientInfo;
        public event onProtocol OnProtocol;
        public event onWelcome OnServerWelcome;
        
        #endregion

        public NetworkClient()
        {
            _protocol = new();
            _mThread = new(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (_socket.Connected)
                    receive();

                Thread.CurrentThread.Abort();
            });
        }

        public void Connect(string hostname, int port, string password)
        {
            adminHost = hostname;
            adminPort = port;
            adminPassword = password;
            Connect();
        }

        public void Connect()
        {
            if (Connect(adminHost, adminPort))
                Start();
        }

        public bool Connect(string host, int port)
        {
            if (adminPassword.Length == 0)
            {
                Console.WriteLine("Can't connect with empty password");
                return false;
            }
            try
            {
                _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                _socket.Connect(host, port);

                sendAdminJoin();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while trying to connect to: " + host);
                return false;
            }
            return true;
        }

        public void chatPublic(string msg)
        {
            sendAdminChat(NetworkAction.NETWORK_ACTION_CHAT, DestType.DESTTYPE_BROADCAST, 0, msg, 0);
        }

        public void Start()
        {
            _mThread.Start();
        }

        public void receive()
        {
            try
            {
                var p = NetworkInputThread.GetNext(_socket);
                delegatePacket(p);
            }
            catch (Exception)
            {
            }
        }

        private void delegatePacket(Packet p)
        {
            var t = GetType();
            var dispatchName = p.GetPacketType().getDispatchName();

            var method = t.GetMethod(dispatchName);

            var mis = t.GetMethods();

            try
            {
                if (method is null)
                {
                    Console.WriteLine($"Method with name {dispatchName} not found!");
                }
                method?.Invoke(this, new object[] { p });
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Method: " + dispatchName);
            }
        }

        #region Polls
        
        public void pollCmdNames()
        {
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_CMD_NAMES);
        }

        /// <summary>
        /// Poll for information on a client if clientId is passed
        /// </summary>
        /// <param name="clientId">Optional parameter specifying the Client ID to get info on</param>
        public void pollClientInfos(long clientId = long.MaxValue)
        {
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, clientId);
        }

        /// <summary>
        /// Poll for information on a company if companyId is passed
        /// </summary>
        /// <param name="companyId">Optional parameter specifying the Company ID to get info on</param>
        public void pollCompanyInfos(long companyId = long.MaxValue)
        {
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_INFO, companyId);
        }

        public void pollCompanyStats()
        {
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_STATS);
        }

        public void pollCompanyEconomy()
        {
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_ECONOMY);
        }

        public void pollDate()
        {
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_DATE);
        }
        
        #endregion

        #region Send Packets

        public void sendAdminJoin()
        {
            var p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_JOIN);

            p.WriteString(adminPassword);
            p.WriteString(botName);
            p.WriteString(botVersion);

            NetworkOutputThread.Append(p);
        }

        public void sendAdminChat(NetworkAction action, DestType type, long dest, string msg, long data)
        {
            var p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_CHAT);
            p.WriteUint8((short)action);
            p.WriteUint8((short)type);
            p.WriteUint32(dest);

            msg = (msg.Length > 900) ? msg.Substring(0, 900) : msg;

            p.WriteString(msg);

            p.WriteUint64(data);
            NetworkOutputThread.Append(p);
        }

        public void sendAdminGameScript(string command)
        {
            var p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_GAMESCRIPT);

            p.WriteString(command); // JSON encode
            NetworkOutputThread.Append(p);
        }

        public void sendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            if (_protocol.IsSupported(type, freq) == false)
                throw new ArgumentException("The server does not support " + freq + " for " + type);

            var p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY);
            p.WriteUint16((int)type);
            p.WriteUint16((int)freq);

            NetworkOutputThread.Append(p);
        }

        public void sendAdminPoll(AdminUpdateType type, long data = 0)
        {
            if (_protocol.IsSupported(type, AdminUpdateFrequency.ADMIN_FREQUENCY_POLL) == false)
                throw new ArgumentException("The server does not support polling for " + type);

            var p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_POLL);
            p.WriteUint8((short)type);
            p.WriteUint32(data);

            NetworkOutputThread.Append(p);
        }

        #endregion

        #region Receive Packets
        
        public void receiveServerClientInfo(Packet p)
        {
            var client = new Client(p.ReadUint32())
            {
                NetworkAddress = p.ReadString(),
                Name = p.ReadString(),
                Language = (NetworkLanguage) p.ReadUint8(),
                JoinDate = new(p.ReadUint32()),
                CompanyId = p.ReadUint8()
            };
            Console.WriteLine($@"{nameof(receiveServerClientInfo)}: ID {client.Id}; Name: {client.Name}");
            OnClientInfo?.Invoke(client);
        }

        public void receiveServerClientError(Packet p)
        {
            //_network.Disconnect();
            var error = (NetworkErrorCode) p.ReadUint8();
            Console.WriteLine($"ERROR CAPTURED: {error.ToString()}");
        }
        
        public void receiveServerClientJoin(Packet p)
        {
            var clientId = p.ReadUint32();

            pollClientInfos(clientId);
            Console.WriteLine($"Unknown client joined #{clientId}");
        }

        public void receiveServerProtocol(Packet p)
        {
            _protocol.Version = p.ReadUint8();

            while (p.ReadBool())
            {
                var tIndex = p.ReadUint16();
                var fValues = p.ReadUint16();

                foreach (AdminUpdateFrequency freq in Enum.GetValues(typeof(AdminUpdateFrequency)))
                {
                    var index = fValues & (int)freq;

                    if (index != 0)
                    {
                        _protocol.AddSupport(tIndex, (int)freq);
                    }
                }
            }

            OnProtocol?.Invoke(_protocol);
        }

        public void receiveServerWelcome(Packet p)
        {
            var map = new Map();
            
            var game = new Game();
            
            game.Name = p.ReadString();
            game.GameVersion = p.ReadString();
            game.Dedicated = p.ReadBool();

            map.Name = p.ReadString();
            map.Seed = p.ReadUint32();
            map.Landscape = (Landscape) p.ReadUint8();
            map.StartDate = new(p.ReadUint32());
            map.Width = p.ReadUint16();
            map.Height = p.ReadUint16();

            game.Map = map;

            OnServerWelcome?.Invoke();
        }

        public void receiveServerConsole(Packet p)
        {
            var origin = p.ReadString();
            var message = p.ReadString();
            
            OnChat?.Invoke(origin, message);
        }

        public void receiveServerCmdNames(Packet p)
        {
            while (p.ReadBool())
            {
                var cmdId = p.ReadUint16();
                var cmdName = p.ReadString();
                if(DoCommandName.Enumeration.ContainsKey(cmdName) == false)
                    DoCommandName.Enumeration.Add(cmdName, cmdId);
            }
        }

        public void receiveServerCmdLogging(Packet p)
        {

        }
        
        #endregion
    }
}
