using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Coan_OLD.Networks
{
    public class NetworkClient
    {
        private readonly Network _network;
        private RconBuffer _rconBuffer;

        public NetworkClient(Network network)
        {
            _network = network;
            _rconBuffer = new RconBuffer();
        }

        public void Run()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (_network.IsConnected())
                {
                    Receive();
                }
                
                Thread.CurrentThread.Abort();
            }).Start();
        }

        public void Send(PacketType type)
        {
            var packet = new Packet(_network.Socket, type);
            _network.OutputThread.Append(packet);
        }

        private void Receive()
        {
            var packet = _network.InputThread.GetNext(_network.Socket);
            DelegatePacket(packet);
        }

        private void DelegatePacket(Packet packet)
        {
            var type = GetType();
            var dispatchName = packet.GetPacketType().GetDispatchName();
            //var method = type.GetMethod(dispatchName, BindingFlags.IgnoreCase);

            var method = type.GetMethods().FirstOrDefault(m =>
                string.Equals(m.Name, dispatchName, StringComparison.InvariantCultureIgnoreCase));
            
            if (method is null)
                Console.WriteLine($"Method not found for name: {dispatchName}");

            if (method?.GetParameters().Any() ?? false)
            {
                method.Invoke(this, new object[] {_network.OpenTTD, packet});
            }
            else
            {
                method?.Invoke(this, Array.Empty<object>());
            }
        }

        private void HandleCmdPause(long p1, long p2)
        {
            var ottd = _network.OpenTTD;
            var pm = (PauseMode) p1;
            var paused = p2 != 0;

            ottd.Game.SetPauseMode(pm, paused);
            ottd.OnPause(pm, paused);
        }

        #region Polls

        public void PollDate()
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_DATE);
        }

        public void PollClientInfos()
        {
            PollClientInfo(long.MaxValue);
        }

        public void PollClientInfo(long clientId)
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, clientId);
        }

        public void PollCompanyInfos()
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_INFO, long.MaxValue);
        }

        public void PollCompanyInfo(int companyId)
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_INFO, companyId);
        }

        public void PollCompanyEconomy()
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_ECONOMY);
        }

        public void PollCompanyStats()
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_COMPANY_STATS);
        }

        public void PollCmdNames()
        {
            SendAdminPoll(AdminUpdateType.ADMIN_UPDATE_CMD_NAMES);
        }

        public void SendAdminPoll(AdminUpdateType type, long data = 0)
        {
            //if (!network.getProtocol().isSupported(type, AdminUpdateFrequency.ADMIN_FREQUENCY_POLL))
            //    throw new IllegalArgumentException("The server does not support ADMIN_FREQUENCY_POLL for " + type);

            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_POLL);
            packet.WriteUint8((short) type);
            packet.WriteUint32(data);

            _network.OutputThread.Append(packet);
        }

        #endregion

        #region Send

        public void SendAdminJoin()
        {
            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_JOIN);
            packet.WriteString(_network.OpenTTD.Password);
            packet.WriteString(_network.OpenTTD.BotName);
            packet.WriteString(_network.OpenTTD.BotVersion);

            _network.OutputThread.Append(packet);
        }

        public void SendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            if (_network.Protocol.IsSupported(type, freq))
                throw new ArgumentException($"The server does not support {freq} for {nameof(type)}");

            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY);
            packet.WriteUint16((int) type);
            packet.WriteUint16((int) freq);

            _network.OutputThread.Append(packet);
        }

        public void SendAdminChat(NetworkAction action, DestType type, long dest, string message, long data)
        {
            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_CHAT);
            packet.WriteUint8((short) action);
            packet.WriteUint8((short) type);
            packet.WriteUint32(dest);

            message = message.Trim();
            if (message.Length >= 900)
            {
                return;
            }

            packet.WriteString(message);
            packet.WriteUint64(data);

            _network.OutputThread.Append(packet);
        }

        public void SendAdminQuit()
        {
            Send(PacketType.ADMIN_PACKET_ADMIN_QUIT);
            _network.Disconnect();
        }

        public void SendAdminRcon(string command)
        {
            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_RCON);
            packet.WriteString(command);
            _network.OutputThread.Append(packet);
        }

        public void SendAdminGameScript(string json)
        {
            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_GAMESCRIPT);
            packet.WriteString(json);
            _network.OutputThread.Append(packet);
        }

        public void SendAdminPing(long d1)
        {
            var packet = new Packet(_network.Socket, PacketType.ADMIN_PACKET_ADMIN_PING);
            packet.WriteUint32(d1);
            _network.OutputThread.Append(packet);
        }

        #endregion

        #region Receive
     
        public void ReceiveServerFull(OpenTTD ottd, Packet p)
        {
            _network.Disconnect();
            ottd.OnServerFull();
        }

        public void ReceiveServerBanned(OpenTTD ottd, Packet p)
        {
            _network.Disconnect();
            ottd.OnServerBanned();
        }

        public void ReceiveServerError(OpenTTD ottd, Packet p)
        {
            _network.Disconnect();
            var error = (NetworkErrorCode) p.ReadUint8();
            ottd.OnServerError(error);
        }

        public void ReceiveServerWelcome(OpenTTD ottd, Packet p)
        {
            var game = new Game();
            var map = new Map();

            game.Name = p.ReadString();
            game.GameVersion = p.ReadString();
            game.Dedicated = p.ReadBool();

            map.Name = p.ReadString();
            map.Seed = p.ReadUint32();
            map.Landscape = (Landscape) p.ReadUint8();
            map.StartDate = new GameDate(p.ReadUint32());
            map.Width = p.ReadUint16();
            map.Height = p.ReadUint16();

            game.Map = map;

            ottd.OnServerWelcome(game);
        }

        public void ReceiveServerDate(OpenTTD ottd, Packet p)
        {
            var date = new GameDate(p.ReadUint32());
            ottd.Game.Map.CurrentDate = date;
            ottd.OnServerDate(date);
        }

        public void ReceiveServerClientJoin(OpenTTD ottd, Packet p)
        {
            var pool = ottd.Pool;
            var clientId = p.ReadUint32();

            var client = pool.ClientPool.FirstOrDefault(c => c.Id == clientId);
            if (client is not null)
            {
                ottd.OnClientJoin(client);
                return;
            }

            PollClientInfo(clientId);
            Console.WriteLine($"Unknown client joined #{clientId}");
        }

        public void ReceiveServerClientInfo(OpenTTD ottd, Packet p)
        {
            var client = new Client(p.ReadUint32())
            {
                NetworkAddress = p.ReadString(),
                Name = p.ReadString(),
                Language = (NetworkLanguage) p.ReadUint8(),
                JoinDate = new GameDate(p.ReadUint32()),
                CompanyId = p.ReadUint8()
            };


            ottd.Pool.ClientPool.TryAdd(client);
            ottd.OnClientInfo(client);
        }

        public void ReceiveServerClientUpdate(OpenTTD ottd, Packet p)
        {
            var clientId = p.ReadUint32();
            var client =  ottd.Pool.ClientPool.FirstOrDefault(c => c.Id == clientId);
            
            if (client is not null)
            {
                client.Name = p.ReadString();
                client.CompanyId = p.ReadUint8();

                ottd.OnClientUpdate(client);
                return;
            }

            PollClientInfo(clientId);
            Console.WriteLine($"Unknown client update #{clientId}");
        }

        public void ReceiveServerClientQuit(OpenTTD ottd, Packet p)
        {
            var clientId = p.ReadUint32();

            if (ottd.Pool.ClientPool.Any(c => c.Id == clientId))
            {
                ottd.Pool.ClientPool.TryRemove(clientId, out var client);
                ottd.OnClientQuit(client);
                return;
            }

            Console.WriteLine($"Unknown client quit #{clientId}");
        }

        public void ReceiveServerClientError(OpenTTD ottd, Packet p)
        {
            var clientId = p.ReadUint32();
            var error = (NetworkErrorCode) p.ReadUint8();
            
            if (ottd.Pool.ClientPool.Any(c => c.Id == clientId))
            {
                ottd.Pool.ClientPool.TryRemove(clientId, out var client);
                ottd.OnClientError(client, error);
                return;
            }
            
            Console.WriteLine($"Unknown client error #{clientId}");
        }

        public void ReceiveServerCompanyNew(OpenTTD ottd, Packet p)
        {
            _ = p.ReadUint32();
            var companyId = p.ReadUint8();

            var company = ottd.Pool.CompanyPool.FirstOrDefault(c => c.Id == companyId);
            if (company is not null)
            {
                ottd.OnCompanyNew(company);
                return;
            }

            PollCompanyInfo(companyId);
            Console.WriteLine($"Unknown company new #{companyId}");
        }

        public void ReceiveServerCompanyInfo(OpenTTD ottd, Packet p)
        {
            var company = new Company(p.ReadUint8())
            {
                Name = p.ReadString(),
                President = p.ReadString(),
                Color = (Colors) p.ReadUint8(),
                IsPassworded = p.ReadBool(),
                Inaugurated = p.ReadUint32(),
                IsAI = p.ReadBool()
            };

            ottd.Pool.CompanyPool.TryAdd(company);
            ottd.OnCompanyInfo(company);
        }

        public void ReceiveServerCompanyUpdate(OpenTTD ottd, Packet p)
        {
            var companyId = p.ReadUint8();
            var company = ottd.Pool.CompanyPool.FirstOrDefault(c => c.Id == companyId);
            
            if (company is not null)
            {
                company.Name = p.ReadString();
                company.President = p.ReadString();
                company.Color = (Colors) p.ReadUint8();
                company.IsPassworded = p.ReadBool();
                company.Bankruptcy = p.ReadUint8();

                for (short i = 0; i < 4; i++)
                {
                    company.Shares[i] = p.ReadUint8();
                }

                ottd.OnCompanyUpdate(company);
                return;
            }

            PollCompanyInfo(companyId);
            Console.WriteLine($"Unknown company update #{companyId}");
        }

        public void ReceiveServerCompanyEconomy(OpenTTD ottd, Packet p)
        {
            var companyId = p.ReadUint8();
            var company = ottd.Pool.CompanyPool.FirstOrDefault(c => c.Id == companyId);

            if (company is not null)
            {
                var tmpCurEconomy = new Economy
                {
                    Date = ottd.Game.Map.CurrentDate,
                    Money = p.ReadUint64(),
                    Loan = p.ReadUint64(),
                    Income = p.ReadUint64()
                };

                var e1 = new Economy
                {
                    Date = tmpCurEconomy.Date.PreviousQuarter(),
                    Cargo = p.ReadUint16(),
                    Value = p.ReadUint64(),
                    Performance = p.ReadUint16()
                };

                var e2 = new Economy
                {
                    Date = e1.Date.PreviousQuarter(),
                    Cargo = p.ReadUint16(),
                    Value = p.ReadUint64(),
                    Performance = p.ReadUint16()
                };

                if (company.CurrentEconomy.IsSameQuarter(e1))
                {
                    e1.Money = company.CurrentEconomy.Money;
                    e1.Loan = company.CurrentEconomy.Loan;
                    e1.Income = company.CurrentEconomy.Income;
                }

                company.CurrentEconomy = tmpCurEconomy;

                /* TODO: store e1 and e2 with company economy history */

                ottd.OnCompanyEconomy(company);
                return;
            }

            PollCompanyInfo(companyId);
            Console.WriteLine($"Unknown company economy #{companyId}");
        }

        public void ReceiveServerCompanyStats(OpenTTD ottd, Packet p)
        {
            var vehicleTypes = Enum.GetValues<VehicleType>();
            var companyId = p.ReadUint8();
            var company = ottd.Pool.CompanyPool.FirstOrDefault(c => c.Id == companyId);

            if (company is not null)
            {
                foreach (var vehicle in vehicleTypes)
                {
                    company.Vehicles.TryAdd(vehicle, p.ReadUint16());
                }
                
                foreach (var vehicle in vehicleTypes)
                {
                    company.Stations.TryAdd(vehicle, p.ReadUint16());
                }

                ottd.OnCompanyStats(company);
                return;
            }

            PollCompanyInfo(companyId);
            Console.WriteLine($"Unknown company stats #{companyId}");
        }

        public void ReceiveServerCompanyRemove(OpenTTD ottd, Packet p)
        {
            var companyId = p.ReadUint8();

            var crr = (AdminCompanyRemoveReason)p.ReadUint8();

            if (ottd.Pool.CompanyPool.Any(c => c.Id == companyId))
            {
                ottd.Pool.CompanyPool.TryRemove(companyId, out var company);
                ottd.OnCompanyRemove(company, crr);
            }

            Console.WriteLine($"Unknown company removed #{companyId}");
        }

        public void ReceiveServerChat(OpenTTD ottd, Packet p)
        {
            var action = (NetworkAction) p.ReadUint8();
            var dest = (DestType) p.ReadUint8();
            var clientId = p.ReadUint32();
            var message = p.ReadString();
            BigInteger data = p.ReadUint64();

            var client = ottd.Pool.ClientPool.FirstOrDefault(c => c.Id == clientId);
            if (client is not null)
            {
                ottd.OnChat(action, dest, client, message, data);
                return;
            }

            Console.WriteLine($"Unknown client chat #{clientId}");
        }

        public void ReceiveServerNewgame(OpenTTD openttd, Packet p)
        {
            openttd.OnNewGame();
        }

        public void ReceiveServerShutdown(OpenTTD openttd, Packet p)
        {
            _network.Disconnect();
            openttd.OnShutdown();
        }

        public void ReceiveServerRcon(OpenTTD openttd, Packet p)
        {
            if (_rconBuffer.IsEOR())
            {
                _rconBuffer = new RconBuffer();
            }

            var colour = (Colors) p.ReadUint16();
            var message = p.ReadString();

            _rconBuffer.Add(message, colour);
        }

        public void ReceiveServerRconEnd(OpenTTD ottd, Packet p)
        {
            _rconBuffer.SetEOR();

            ottd.OnRcon(_rconBuffer);
            _rconBuffer = new RconBuffer();
        }

        public void ReceiveServerProtocol(OpenTTD ottd, Packet p)
        {
            var protocol = _network.Protocol;
            protocol.Version = p.ReadUint8();

            while (p.ReadBool())
            {
                var tIndex = p.ReadUint16();
                var fValues = p.ReadUint16();

                foreach (AdminUpdateFrequency freq in Enum.GetValues(typeof(AdminUpdateFrequency)))
                {
                    var index = fValues & (int)freq;
                    if (index != 0)
                    {
                        protocol.AddSupport(tIndex, (int)freq);
                    }
                }
            }

            _network.OpenTTD.OnProtocol(protocol);
        }

        public void ReceiveServerConsole(OpenTTD ottd, Packet p)
        {
            var origin = p.ReadString();
            var message = p.ReadString();

            ottd.OnConsole(origin, message);
        }

        public void ReceiveServerCmdNames(OpenTTD _, Packet p)
        {
            while (p.ReadBool())
            {
                var cmdId = p.ReadUint16();
                var cmdName = p.ReadString();

                //DoCommandName.Create(cmdName, cmdId);
            }
        }

        public void ReceiveServerCmdLogging(OpenTTD ottd, Packet p)
        {
            var clientId = p.ReadUint32();
            var companyId = p.ReadUint8();
            var commandId = p.ReadUint16();
            var p1 = p.ReadUint32();
            var p2 = p.ReadUint32();
            var tile = p.ReadUint32();
            var text = p.ReadString();
            var frame = p.ReadUint32();

            var client = ottd.Pool.ClientPool.FirstOrDefault(c => c.Id == clientId);
            if (client is null)
            {
                PollClientInfo(clientId);
                return;
            }

            var company = ottd.Pool.CompanyPool.FirstOrDefault(c => c.Id == commandId);
            if (company is null)
            {
                PollCompanyInfo(companyId);
                return;
            }

            /*
            var command = DoCommandName.ValueOf(commandId);

            if (command == null)
            {
                PollCmdNames();
                return;
            }

            if ("CmdPause".Equals(command.ToString()))
            {
                HandleCmdPause(p1, p2);
            }

            ottd.OnCmdLogging(client, company, command, p1, p2, tile, text, frame);
            */
        }

        public void ReceiveServerGameScript(OpenTTD ottd, Packet p)
        {
            var json = p.ReadString();
            ottd.OnGameScript(json);
        }

        public void ReceiveServerPong(OpenTTD ottd, Packet p)
        {
            var d1 = p.ReadUint32();
            ottd.OnPong(d1);
        }
        
        #endregion
    }
}