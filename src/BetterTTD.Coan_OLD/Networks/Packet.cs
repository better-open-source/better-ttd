using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Coan_OLD.Networks
{
    public class Packet
    {
        private const int SendMtu = 1460;
        private const int PosPacketType = 2;

        private int _pos;
        private PacketType _type;
        private readonly byte[] _buf;
        private readonly Socket _socket;

        public Packet(Socket socket, PacketType type)
        {
            _socket = socket;
            _buf = new byte[SendMtu];
            SetPacketType(type);
            _pos = PosPacketType + 1;
        }

        public Packet(Socket socket)
        {
            _socket = socket;
            _buf = new byte[2];
            
            if (socket.Connected == false)
                throw new("Socket closed");

            switch (socket.Receive(_buf))
            {
                case { } n when n > SendMtu:
                    throw new IndexOutOfRangeException("Packet length claims to be greater than SEND_MTU");
                case { } n when n == 0:
                    throw new SocketException((int) SocketError.NoData);
            }
            
            _pos = PosPacketType + 1;
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
            _buf[PosPacketType] = (byte) type;
            _type = type;
        }

        public PacketType GetPacketType()
        {
            if (_type == 0)
            {
                if (PosPacketType == _buf.Length)
                {
                    _type = (PacketType) (_buf.Last() & 0xFF);
                }
                else
                {
                    _type = (PacketType) (_buf[PosPacketType] & 0xFF);
                }
            }

            return _type;
        }

        public bool IsSocketCloseIndicator() =>
            GetPacketType() switch
            {
                PacketType.ADMIN_PACKET_SERVER_FULL or
                PacketType.ADMIN_PACKET_SERVER_BANNED or
                PacketType.ADMIN_PACKET_SERVER_ERROR or
                PacketType.ADMIN_PACKET_SERVER_SHUTDOWN => true,
                _ => false
            };

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