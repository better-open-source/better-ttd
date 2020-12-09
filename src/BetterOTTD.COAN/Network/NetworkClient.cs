﻿using System;
using System.Threading;
using System.Net.Sockets;
using BetterOTTD.COAN.Common;
using BetterOTTD.COAN.Network;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;

namespace COAN
{
    public class NetworkClient
    {
        private Protocol protocol;
        private Socket socket;
        private Thread mThread;

        public string botName = "Bot Name";
        public string botVersion = "BOT VERSION";

        public string adminHost = "";
        public string adminPassword = "";
        public int adminPort = 3978;

        #region Delegates
        /// <summary>
        /// Fired when messages are recieved
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dest"></param>
        /// <param name="clientId">The ID of the Client who sent the message</param>
        /// <param name="message">The actual chat message</param>
        /// <param name="data"></param>
        public delegate void onChat(NetworkAction action, DestType dest, long clientId, string message, long data);
        /// <summary>
        /// Fired when Client information is received
        /// </summary>
        /// <param name="client">The Client information</param>
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
            this.protocol = new Protocol();
            mThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (IsConnected() == true)
                    receive();

                Thread.CurrentThread.Abort();
            });
        }

        public void Connect(string hostname, int port, string password)
        {
            this.adminHost = hostname;
            this.adminPort = port;
            this.adminPassword = password;
            this.Connect();
        }

        public void Connect()
        {
            if (Connect(this.adminHost, this.adminPort) == true)
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
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                this.socket.Connect(host, port);

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
            return socket.Connected;
        }

        public void Start()
        {
            mThread.Start();
        }

        public void receive()
        {
            try
            {
                Packet p = NetworkInputThread.getNext(getSocket());
                delegatePacket(p);
            }
            catch (Exception)
            {
            }
        }

        public void delegatePacket(Packet p)
        {
            Type t = this.GetType();
            String dispatchName = p.getType().getDispatchName();

            System.Reflection.MethodInfo method = t.GetMethod(dispatchName);

            System.Reflection.MethodInfo[] mis = t.GetMethods();

            try
            {
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
            Packet p = new Packet(getSocket(), PacketType.ADMIN_PACKET_ADMIN_JOIN);

            p.WriteString(adminPassword);
            p.WriteString(botName);
            p.WriteString(botVersion);

            NetworkOutputThread.append(p);
        }

        public void sendAdminChat(NetworkAction action, DestType type, long dest, String msg, long data)
        {
            Packet p = new Packet(getSocket(), PacketType.ADMIN_PACKET_ADMIN_CHAT);
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
            Packet p = new Packet(getSocket(), PacketType.ADMIN_PACKET_ADMIN_GAMESCRIPT);


            p.WriteString(command); // JSON encode
            NetworkOutputThread.append(p);
        }

        public void sendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            if (getProtocol().isSupported(type, freq) == false)
                throw new ArgumentException("The server does not support " + freq + " for " + type);

            Packet p = new Packet(getSocket(), PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY);
            p.writeUint16((int)type);
            p.writeUint16((int)freq);

            NetworkOutputThread.append(p);
        }

        public void sendAdminPoll(AdminUpdateType type, long data = 0)
        {
            if (getProtocol().isSupported(type, AdminUpdateFrequency.ADMIN_FREQUENCY_POLL) == false)
                throw new ArgumentException("The server does not support polling for " + type);

            Packet p = new Packet(getSocket(), PacketType.ADMIN_PACKET_ADMIN_POLL);
            p.writeUint8((short)type);
            p.writeUint32(data);

            NetworkOutputThread.append(p);
        }

        #endregion

        #region Receive Packets
        public void receiveServerClientInfo(Packet p)
        {
            Client client = new Client(p.readUint32())
            {
                NetworkAddress = p.readString(), Name = p.readString()
            };

            //client.language = NetworkLanguage.valueOf(p.readUint8());
            p.readUint8();
            client.JoinDate = new GameDate(p.readUint32());
            client.CompanyId = p.readUint8();

            Console.WriteLine($@"{nameof(receiveServerClientInfo)}: ID {client.Id}; Name: {client.Name}");

            OnClientInfo?.Invoke(client);
        }

        public void receiveServerProtocol(Packet p)
        {
            Protocol protocol = getProtocol();

            protocol.version = p.readUint8();

            while (p.readBool())
            {
                int tIndex = p.readUint16();
                int fValues = p.readUint16();

                foreach (AdminUpdateFrequency freq in Enum.GetValues(typeof(AdminUpdateFrequency)))
                {
                    int index = fValues & (int)freq;

                    if (index != 0)
                    {
                        protocol.addSupport(tIndex, (int)freq);
                    }
                }
            }

            OnProtocol?.Invoke(protocol);
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
            NetworkAction action = (NetworkAction) p.readUint8();
            DestType dest = (DestType) p.readUint8();
            long clientId = p.readUint32();
            String message = p.readString();
            long data = p.readUint64();

            OnChat?.Invoke(action, dest, clientId, message, data);
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

        #region Getters
        public Socket getSocket()
        {
            return this.socket;
        }

        public Protocol getProtocol()
        {
            return this.protocol;
        }
        #endregion

    }
}
