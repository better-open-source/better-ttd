using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using Akka.Actor;
using BetterTTD.Network;

namespace BetterTTD.Actors
{
    public class NetworkClientCoordinatorActor : ReceiveActor
    {
        private readonly List<Socket> _sockets;
        
        public NetworkClientCoordinatorActor()
        {
            _sockets = new();
        }
    }

    public class NetworkClientActor : ReceiveActor
    {
        private Socket _socket;

        public NetworkClientActor()
        {
            Receive<ConnectMessage>(ConnectMessageHandler);
        }

        private void ConnectMessageHandler(ConnectMessage msg)
        {
            if (_socket?.Connected ?? false)
                return;

            var (host, port) = msg;
            
            _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            _socket.Connect(host, port);
        }
    }

    public class NetworkInputActor : ReceiveActor, IWithTimers
    {
        private BlockingCollection<Packet> _packets;
        
        public NetworkInputActor()
        {
            _packets = new();
        }

        public ITimerScheduler Timers { get; set; }
    }

    public class NetworkOutputActor : ReceiveActor, IWithTimers
    {
        private readonly BlockingCollection<Packet> _packets;
        
        public NetworkOutputActor()
        {
            _packets = new();
            Receive<AddOutputMessage>(AddOutputMessageHandler);
        }

        private void AddOutputMessageHandler(AddOutputMessage msg)
        {
            _packets.Add(msg.Packet);
        }

        public ITimerScheduler Timers { get; set; }
    }
    
    public record ConnectMessage(string Host, int Port);
    public record GetNextInputMessage(Packet Packet);
    public record AddOutputMessage(Packet Packet);
}