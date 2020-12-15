using System.Net.Sockets;
using System.Text;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Network
{
    public class Packet
    {
        private const int SendMtu = 1460;
        private const int PosPacketType = 2;

        private int _pos;
        private readonly byte[] _buf = new byte[SendMtu];

        private PacketType _type;

        public Socket Socket { get; }
        
        public Packet(Socket socket, PacketType type)
        {
            Socket = socket;
            _buf = new byte[SendMtu];
            SetType(type);
            _pos = PosPacketType + 1;
        }

        public Packet(Socket socket)
        {
            Socket = socket;

            if (socket.Connected == false)
                return;

            var length = socket.Receive(_buf);

            if (length == 0)
                throw new SocketException();

            _pos = PosPacketType + 1;
        }

        public int Length()
        {
            var b1 = _buf[0] & 0xFF;
            var b2 = _buf[1] & 0xFF;

            var r = b1 + (b2 << 8);

            return r;
        }

        private void SetType(PacketType type)
        {
            _buf[PosPacketType] = (byte)type;
        }

        public PacketType GetPacketType()
        {
            if (_type != 0) 
                return _type;
            var type = (PacketType)(_buf[PosPacketType] & 0xFF);
            return _type = type;
        }

        public void WriteString(string str)
        {
            foreach (var b in Encoding.Default.GetBytes(str))
            {
                _buf[_pos++] = b;
            }
            _buf[_pos++] = (byte)'\0';
        }

        public void WriteUint8(short n)
        {
            _buf[_pos++] = (byte)n;
        }

        public void WriteUint16(int n)
        {
            _buf[_pos++] = (byte)n;
            _buf[_pos++] = (byte)(n >> 8);
        }

        public void WriteUint32(long n)
        {
            _buf[_pos++] = (byte)n;
            _buf[_pos++] = (byte)(n >> 8);
            _buf[_pos++] = (byte)(n >> 16);
            _buf[_pos++] = (byte)(n >> 24);
        }

        public void WriteUint64(long n)
        {
            _buf[_pos++] = (byte)n;
            _buf[_pos++] = (byte)(n >> 8);
            _buf[_pos++] = (byte)(n >> 16);
            _buf[_pos++] = (byte)(n >> 24);
            _buf[_pos++] = (byte)(n >> 32);
            _buf[_pos++] = (byte)(n >> 40);
            _buf[_pos++] = (byte)(n >> 48);
            _buf[_pos++] = (byte)(n >> 56);
        }

        public int ReadUint8()
        {
            return _buf[_pos++] & 0xFF;
        }

        public int ReadUint16()
        {
            var n = _buf[_pos++] & 0xFF;
            n += (_buf[_pos++] & 0xFF) << 8;

            return n;
        }

        public long ReadUint32()
        {
            long n = _buf[_pos++] & 0xFF;
            n += (_buf[_pos++] & 0xFF) << 8;
            n += (_buf[_pos++] & 0xFF) << 16;
            n += (_buf[_pos++] & 0xFF) << 24;

            return n;
        }

        public long ReadUint64()
        {
            long l = 0;
            l += _buf[_pos++] & 0xFF;
            l += (long)(_buf[_pos++] & 0xFF) << 8;
            l += (long)(_buf[_pos++] & 0xFF) << 16;
            l += (long)(_buf[_pos++] & 0xFF) << 24;
            l += (long)(_buf[_pos++] & 0xFF) << 32;
            l += (long)(_buf[_pos++] & 0xFF) << 40;
            l += (long)(_buf[_pos++] & 0xFF) << 48;
            l += (long)(_buf[_pos++] & 0xFF) << 56;

            return l;
        }

        public string ReadString()
        {
            var startIdx = _pos;

            while (_buf[_pos++] != (byte)'\0') ;

            var str = Encoding.GetEncoding("UTF-8").GetString(_buf);
            str = str.Substring(startIdx, _pos - startIdx);
            
            return str;
        }

        public bool ReadBool()
        {
            return (_buf[_pos++] & 0xFF) > 0;
        }

        public void Send()
        {
            _buf[0] = (byte)_pos;
            _buf[1] = (byte)(_pos >> 8);

            Socket.Send(_buf, _pos, SocketFlags.None);
        }
    }
}
