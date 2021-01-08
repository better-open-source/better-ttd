using System.Net.Sockets;
using BetterTTD.Domain.Enums;
using CSharpFunctionalExtensions;

namespace BetterTTD.Network
{
    public partial class Packet
    {
        private const int SendMtu = 1460;
        private const int PosPacketType = 2;

        private int _pos;
        private readonly byte[] _buf;
        private PacketType _type;

        private Packet(byte[] buf)
        {
            _buf = buf;
            _pos = PosPacketType + 1;
        }
        
        public Packet(PacketType type)
        {
            _buf = new byte[SendMtu];
            SetType(type);
            _pos = PosPacketType + 1;
        }

        public static Result<Packet> Create(Socket socket)
        {
            var buf = new byte[SendMtu];
            return socket switch
            {
                _ when socket is null => Result.Failure<Packet>("NullError"),
                _ when !socket.Connected => Result.Failure<Packet>("SocketNotConnectedError"),
                _ when socket.Receive(buf) != 0 => new Packet(buf),
                _ => Result.Failure<Packet>("Socket unhandled error.")
            };
        }

        public PacketType GetPacketType()
        {
            if (_type != 0) 
                return _type;
            var type = (PacketType)(_buf[PosPacketType] & 0xFF);
            return _type = type;
        }

        public void SendTo(Socket socket)
        {
            _buf[0] = (byte)_pos;
            _buf[1] = (byte)(_pos >> 8);

            socket.Send(_buf, _pos, SocketFlags.None);
        }
        
        private void SetType(PacketType type)
        {
            _buf[PosPacketType] = (byte)type;
        }
    }
}
