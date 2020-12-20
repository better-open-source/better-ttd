using System.Net.Sockets;
using Akka.Actor;
using Akka.Event;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterTTD.Actors.ClientGroup.SenderGroup
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
            _log.Info($"Handled {nameof(SendAdminJoinMessage)}. " +
                      $"Pass:{msg.AdminPassword}; " +
                      $"BotName:{msg.BotName}; " +
                      $"BotVersion:{msg.BotVersion}");

            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_JOIN);

            packet.WriteString(msg.AdminPassword);
            packet.WriteString(msg.BotName);
            packet.WriteString(msg.BotVersion);

            packet.SendTo(_socket);
        }

        private void SendAdminUpdateFrequencyHandler(SendAdminUpdateFrequencyMessage msg)
        {
            _log.Info($"Handled {nameof(SendAdminUpdateFrequencyHandler)}. " +
                      $"Type:{msg.Type}; " +
                      $"Freq:{msg.Freq}.");

            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY);

            packet.WriteUint16((int) msg.Type);
            packet.WriteUint16((int) msg.Freq);

            packet.SendTo(_socket);
        }

        private void SendAdminPollMessageHandler(SendAdminPollMessage msg)
        {
            _log.Info($"Handled {nameof(SendAdminPollMessageHandler)}. " +
                      $"Type:{msg.Type}; " +
                      $"Data:{msg.Data}.");

            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_POLL);

            packet.WriteUint8((short) msg.Type);
            packet.WriteUint32(msg.Data);

            packet.SendTo(_socket);
        }
    }
}