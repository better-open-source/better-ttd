using System;
using System.Net.Sockets;
using System.Threading;
using BetterOTTD.COAN.Common;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;

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
            _protocol = new Protocol();
            _mThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (IsConnected())
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
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

        public Boolean IsConnected()
        {
            return _socket.Connected;
        }

        public void Start()
        {
            _mThread.Start();
        }

        public void receive()
        {
            try
            {
                Packet p = NetworkInputThread.getNext(_socket);
                delegatePacket(p);
            }
            catch (Exception)
            {
            }
        }

        private void delegatePacket(Packet p)
        {
            Type t = GetType();
            String dispatchName = p.getType().getDispatchName();

            System.Reflection.MethodInfo method = t.GetMethod(dispatchName);

            System.Reflection.MethodInfo[] mis = t.GetMethods();

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
            sendAdminPoll(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, companyId);
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
            Packet p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_JOIN);

            p.WriteString(adminPassword);
            p.WriteString(botName);
            p.WriteString(botVersion);

            NetworkOutputThread.append(p);
        }

        public void sendAdminChat(NetworkAction action, DestType type, long dest, String msg, long data)
        {
            Packet p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_CHAT);
            p.writeUint8((short)action);
            p.writeUint8((short)type);
            p.writeUint32(dest);

            msg = (msg.Length > 900) ? msg.Substring(0, 900) : msg;

            p.WriteString(msg);

            p.writeUint64(data);
            NetworkOutputThread.append(p);
        }

        public void sendAdminGameScript(string command)
        {
            Packet p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_GAMESCRIPT);


            p.WriteString(command); // JSON encode
            NetworkOutputThread.append(p);
        }

        public void sendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            if (_protocol.isSupported(type, freq) == false)
                throw new ArgumentException("The server does not support " + freq + " for " + type);

            Packet p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY);
            p.writeUint16((int)type);
            p.writeUint16((int)freq);

            NetworkOutputThread.append(p);
        }

        public void sendAdminPoll(AdminUpdateType type, long data = 0)
        {
            if (_protocol.isSupported(type, AdminUpdateFrequency.ADMIN_FREQUENCY_POLL) == false)
                throw new ArgumentException("The server does not support polling for " + type);

            Packet p = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_POLL);
            p.writeUint8((short)type);
            p.writeUint32(data);

            NetworkOutputThread.append(p);
        }

        #endregion

        #region Receive Packets
        public void receiveServerClientInfo(Packet p)
        {
            var client = new Client(p.readUint32())
            {
                NetworkAddress = p.readString(),
                Name = p.readString(),
                Language = (NetworkLanguage) p.readUint8(),
                JoinDate = new GameDate(p.readUint32()),
                CompanyId = p.readUint8()
            };
            Console.WriteLine($@"{nameof(receiveServerClientInfo)}: ID {client.Id}; Name: {client.Name}");
            OnClientInfo?.Invoke(client);
        }

        public void receiveServerClientError(Packet p)
        {
            //_network.Disconnect();
            var error = (NetworkErrorCode) p.readUint8();
            Console.WriteLine($"ERROR CAPTURED: {error.ToString()}");
        }
        
        public void receiveServerClientJoin(Packet p)
        {
            var clientId = p.readUint32();

            pollClientInfos(clientId);
            Console.WriteLine($"Unknown client joined #{clientId}");
        }

        public void receiveServerProtocol(Packet p)
        {
            _protocol.version = p.readUint8();

            while (p.readBool())
            {
                int tIndex = p.readUint16();
                int fValues = p.readUint16();

                foreach (AdminUpdateFrequency freq in Enum.GetValues(typeof(AdminUpdateFrequency)))
                {
                    int index = fValues & (int)freq;

                    if (index != 0)
                    {
                        _protocol.addSupport(tIndex, (int)freq);
                    }
                }
            }

            OnProtocol?.Invoke(_protocol);
        }

        public void receiveServerWelcome(Packet p)
        {
            Map map = new Map();
            
            Game game = new Game();
            
            game.Name = p.readString();
            game.GameVersion = p.readString();
            game.Dedicated = p.readBool();

            map.Name = p.readString();
            map.Seed = p.readUint32();
            map.Landscape = (Landscape) p.readUint8();
            map.StartDate = new GameDate(p.readUint32());
            map.Width = p.readUint16();
            map.Height = p.readUint16();

            game.Map = map;

            OnServerWelcome?.Invoke();
        }

        public void receiveServerConsole(Packet p)
        {
            var origin = p.readString();
            var message = p.readString();
            
            OnChat?.Invoke(origin, message);
        }

        public void receiveServerCmdNames(Packet p)
        {
            while (p.readBool())
            {
                int cmdId = p.readUint16();
                String cmdName = p.readString();
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
