﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterTTD.Actors
{
    public class DispatcherActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;
        private readonly ActorSelection _bridge;

        public DispatcherActor()
        {
            _log = Context.GetLogger();
            _bridge = Context.ActorSelection("akka://ottd-system/user/BridgeActor");

            Receive<ReceivedBufMessage>(ReceivedBufMessageHandler);
            
            _log.Info("Initialized ");
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new DispatcherActor());
        }
        
        private void ReceivedBufMessageHandler(ReceivedBufMessage msg)
        {
            if (msg?.Packet is null)
            {
                _log.Error($"Invalid parameter: {nameof(Packet)}");
                return;
            }
            
            var dispatchName = msg.Packet.GetPacketType().GetDispatchName();
            _log.Info(dispatchName);
            switch (dispatchName)
            {
                case "receiveServerProtocol":
                    ReceiveServerProtocol(msg.Packet);
                    break;
                case "receiveServerWelcome":
                    ReceiveServerWelcome(msg.Packet);
                    break;
                case "receiveServerCmdNames":
                    ReceiveServerCmdNames(msg.Packet);
                    break;
                case "receiveServerConsole":
                    ReceiveServerConsole(msg.Packet);
                    break;
                case "receiveServerClientInfo":
                    ReceiveServerClientInfo(msg.Packet);
                    break;
                default:
                    _log.Warning($"Unhandled action: {dispatchName}");
                    break;
            }
        }
        
        private void ReceiveServerProtocol(Packet packet)
        {
            var protocol = new Protocol {Version = packet.ReadUint8()};

            while (packet.ReadBool())
            {
                var tIndex = packet.ReadUint16();
                var fValues = packet.ReadUint16();

                foreach (AdminUpdateFrequency freq in Enum.GetValues(typeof(AdminUpdateFrequency)))
                {
                    var index = fValues & (int)freq;

                    if (index != 0)
                    {
                        protocol.AddSupport(tIndex, (int)freq);
                    }
                }
            }

            _bridge.Tell(new OnProtocolMessage(protocol));
        }
        
        private void ReceiveServerWelcome(Packet packet)
        {
            var map = new Map();
            var game = new Game
            {
                Name = packet.ReadString(), 
                GameVersion = packet.ReadString(), 
                Dedicated = packet.ReadBool()
            };

            map.Name = packet.ReadString();
            map.Seed = packet.ReadUint32();
            map.Landscape = (Landscape) packet.ReadUint8();
            map.StartDate = new(packet.ReadUint32());
            map.Width = packet.ReadUint16();
            map.Height = packet.ReadUint16();

            game.Map = map;

            _bridge.Tell(new OnServerWelcomeMessage(game));
        }

        private void ReceiveServerCmdNames(Packet packet)
        {
            var commands = new Dictionary<int, string>();
            while (packet.ReadBool())
            {
                var cmdId = packet.ReadUint16();
                var cmdName = packet.ReadString();
                
                commands.Add(cmdId, cmdName);
            }

            _bridge.Tell(new OnServerCmdNamesMessage(commands));
        }

        private void ReceiveServerConsole(Packet packet)
        {
            var origin = packet.ReadString();
            var message = packet.ReadString();

            _bridge.Tell(new OnServerConsoleMessage(origin, message));
        }

        private void ReceiveServerClientInfo(Packet packet)
        {
            var client = new Client(packet.ReadUint32())
            {
                NetworkAddress = packet.ReadString(),
                Name = packet.ReadString(),
                Language = (NetworkLanguage) packet.ReadUint8(),
                JoinDate = new(packet.ReadUint32()),
                CompanyId = packet.ReadUint8()
            };
            
            _bridge.Tell(new OnServerClientInfoMessage(client));
        }
    }
}