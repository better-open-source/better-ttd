using System.Net.Sockets;
using Akka.Actor;
using Akka.Event;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterTTD.Actors
{
    public sealed class SenderActor : ReceiveActor
    {
        private readonly Socket _socket;
        private readonly ILoggingAdapter _log;
        
        public SenderActor(Socket socket)
        {
            _socket = socket;
            _log = Context.GetLogger();

            Receive<SendAdminJoinMessage>(SendAdminJoinMessageHandler);
            Receive<SendAdminUpdateFrequencyMessage>(SendAdminUpdateFrequencyHandler);
            Receive<SendAdminPollMessage>(SendAdminPollMessageHandler);
            
            _log.Info("Initialized");
        }

        public static Props Props(Socket socket)
        {
            return Akka.Actor.Props.Create(() => new SenderActor(socket));
        }

        private void SendAdminJoinMessageHandler(SendAdminJoinMessage msg)
        {
            var (adminPassword, botName, botVersion) = msg;
            
            _log.Info($"Handled {nameof(SendAdminJoinMessage)}. " +
                      $"Pass:{adminPassword}; " +
                      $"BotName:{botName}; " +
                      $"BotVersion:{botVersion}");
            
            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_JOIN);
            
            packet.WriteString(adminPassword);
            packet.WriteString(botName);
            packet.WriteString(botVersion);

            packet.SendTo(_socket);
        }

        private void SendAdminUpdateFrequencyHandler(SendAdminUpdateFrequencyMessage msg)
        {
            var (type, freq) = msg;

            _log.Info($"Handled {nameof(SendAdminUpdateFrequencyHandler)}. " +
                      $"Type:{type}; " +
                      $"Freq:{freq}.");

            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY);

            packet.WriteUint16((int) type);
            packet.WriteUint16((int) freq);

            packet.SendTo(_socket);
        }
        
        private void SendAdminPollMessageHandler(SendAdminPollMessage msg)
        {
            var (type, data) = msg;
            
            _log.Info($"Handled {nameof(SendAdminPollMessageHandler)}. " +
                      $"Type:{type}; " +
                      $"Data:{data}.");

            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_POLL);
            
            packet.WriteUint8((short)type);
            packet.WriteUint32(data);
            
            packet.SendTo(_socket);
        }
    }
}