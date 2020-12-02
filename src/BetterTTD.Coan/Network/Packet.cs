using System;
using System.Net.Sockets;
using System.Text;
using BetterTTD.Coan.Enums;

namespace BetterTTD.Coan.Network
{
    public class Packet
    {
        private const int SEND_MTU = 1460;
        private const int POS_PACKET_TYPE = 2;

        private int _pos;
        private PacketType? _type;
        private readonly byte[] _buf;
        private readonly Socket _socket;

        public Packet(Socket socket, PacketType type)
        {
            _socket = socket;
            _buf = new byte[SEND_MTU];
            _pos = POS_PACKET_TYPE + 1;
            
            SetPacketType(type);
        }

        public Packet(Socket socket)
        {
            _socket = socket;
            _buf = new byte[2];
            
            if (socket.Connected == false)
                throw new Exception("Socket closed");

            switch (socket.Receive(_buf))
            {
                case { } n when n > SEND_MTU:
                    throw new IndexOutOfRangeException("Packet length claims to be greater than SEND_MTU");
                case { } n when n == 0:
                    throw new SocketException((int) SocketError.NoData);
            }
            
            _pos = POS_PACKET_TYPE + 1;
        }

        public Socket GetSocket()
        {
            return _socket;
        }

        public void Append(byte[] buf)
        {
            Array.Copy(buf, 0, _buf, _buf.LongLength, buf.LongLength);
        } 
        
        private void SetPacketType(PacketType type)
        {
            _buf[POS_PACKET_TYPE] = (byte) type;
            _type = type;
        }

        public PacketType GetType()
        {
            _type ??= (PacketType) (_buf[POS_PACKET_TYPE] & 0xFF);
            return (PacketType) _type;
        }
        
        public void Send()
        {
            _buf[0] = (byte) _pos;
            _buf[1] = (byte) (_pos >> 8);

            _socket.Send(_buf, _pos, SocketFlags.None);
        }

        public int Length()
        {
            var byte1 = _buf[0] & 0xFF;
            var byte2 = _buf[1] & 0xFF;
            var res = byte1 + (byte2 << 8);
            return res;
        }
        
        #region WriteBytes

        public void WriteBool (bool b)
        {
            _buf[_pos++] = b ? 1 : 0;
        }
        
        public void WriteString(string str)
        {
            foreach (var @byte in Encoding.Default.GetBytes(str))
            {
                _buf[_pos++] = @byte;
            }

            _buf[_pos++] = (byte)'\0';
        }

        public void WriteUint8(short n)
        {
            _buf[_pos++] = (byte) n;
        }

        public void WriteUint16(int n)
        {
            _buf[_pos++] = (byte) n;
            _buf[_pos++] = (byte) (n >> 8);
        }

        public void WriteUint32(long n)
        {
            _buf[_pos++] = (byte) n;
            _buf[_pos++] = (byte) (n >> 8);
            _buf[_pos++] = (byte) (n >> 16);
            _buf[_pos++] = (byte) (n >> 24);
        }

        public void WriteUint64(long n)
        {
            _buf[_pos++] = (byte) n;
            _buf[_pos++] = (byte) (n >> 8);
            _buf[_pos++] = (byte) (n >> 16);
            _buf[_pos++] = (byte) (n >> 24);
            _buf[_pos++] = (byte) (n >> 32);
            _buf[_pos++] = (byte) (n >> 40);
            _buf[_pos++] = (byte) (n >> 48);
            _buf[_pos++] = (byte) (n >> 56);
        }

        #endregion

        #region ReadBytes

        public bool ReadBool()
        {
            return (_buf[_pos++] & 0xFF) > 0;
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
            l +=  _buf[_pos++] & 0xFF;
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

            var result = Encoding
                .GetEncoding("UTF-8")
                .GetString(_buf)
                .Substring(startIdx, _pos - startIdx);
            return result;
        }

        #endregion
    }
}