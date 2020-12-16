using System;
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

            _bridge.Tell(new OnServerWelcomeMessage());
        }
    }
}