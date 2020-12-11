using System;
using System.Net.Sockets;
using System.Text;
using BetterTTD.Domain.Enums;

namespace BetterOTTD.COAN.Network
{
    public class Packet
    {
        private const int SEND_MTU = 1460;
        private const int POS_PACKET_TYPE = 2;

        int pos = 0;
        byte[] buf = new byte[SEND_MTU];

        Socket socket;
        PacketType type;

        public Packet(Socket socket, PacketType type)
        {
            this.socket = socket;
            buf = new byte[SEND_MTU];
            SetType(type);
            pos = POS_PACKET_TYPE + 1;
            
        }

        public Packet(Socket socket)
        {
            this.socket = socket;

            if (socket.Connected == false)
                return;

            int leng = socket.Receive(buf);

            if (leng == 0)
                throw new SocketException();

            pos = POS_PACKET_TYPE + 1;
        }

        public int length()
        {
            int b1 = buf[0] & 0xFF;
            int b2 = buf[1] & 0xFF;

            int r = (b1 + (b2 << 8));

            return r;
        }

        public Socket getSocket()
        {
            return socket;
        }

        void SetType(PacketType type)
        {
            buf[POS_PACKET_TYPE] = (byte)type;
        }

        public PacketType getType()
        {
            if (type == 0)
            {
                PacketType t = (PacketType)(buf[POS_PACKET_TYPE] & 0xFF);
                type = t;// PacketType.valueOf(this.buf[POS_PACKET_TYPE] & 0xFF);
            }

            return type;
        }

        public void WriteString(string str)
        {
            foreach (byte b in Encoding.Default.GetBytes(str))
            {
                buf[pos++] = b;
            }
            buf[pos++] = (byte)'\0';

        }

        public void writeUint8(short n)
        {
            buf[pos++] = (byte)n;
        }

        public void writeUint16(int n)
        {
            buf[pos++] = (byte)n;
            buf[pos++] = (byte)(n >> 8);
        }

        public void writeUint32(long n)
        {
            buf[pos++] = (byte)n;
            buf[pos++] = (byte)(n >> 8);
            buf[pos++] = (byte)(n >> 16);
            buf[pos++] = (byte)(n >> 24);
        }

        public void writeUint64(long n)
        {
            buf[pos++] = (byte)n;
            buf[pos++] = (byte)(n >> 8);
            buf[pos++] = (byte)(n >> 16);
            buf[pos++] = (byte)(n >> 24);
            buf[pos++] = (byte)(n >> 32);
            buf[pos++] = (byte)(n >> 40);
            buf[pos++] = (byte)(n >> 48);
            buf[pos++] = (byte)(n >> 56);
        }

        public int readUint8()
        {
            return (buf[pos++] & 0xFF);
        }

        public int readUint16()
        {
            int n = buf[pos++] & 0xFF;
            n += (buf[pos++] & 0xFF) << 8;

            return n;
        }

        public long readUint32()
        {
            long n = buf[pos++] & 0xFF;
            n += (buf[pos++] & 0xFF) << 8;
            n += (buf[pos++] & 0xFF) << 16;
            n += (buf[pos++] & 0xFF) << 24;

            return n;
        }

        public long readUint64()
        {
            long l = 0;
            l += (long)(buf[pos++] & 0xFF);
            l += (long)(buf[pos++] & 0xFF) << 8;
            l += (long)(buf[pos++] & 0xFF) << 16;
            l += (long)(buf[pos++] & 0xFF) << 24;
            l += (long)(buf[pos++] & 0xFF) << 32;
            l += (long)(buf[pos++] & 0xFF) << 40;
            l += (long)(buf[pos++] & 0xFF) << 48;
            l += (long)(buf[pos++] & 0xFF) << 56;

            return l;
        }

        public String readString()
        {
            String str = "";
            int startIdx = pos;

            while (buf[pos++] != (byte)'\0') ;

            int endIdx = pos - startIdx - 1;

            str = Encoding.GetEncoding("UTF-8").GetString(buf);
            str = str.Substring(startIdx, (pos - startIdx));
            
            return str;
        }


        public bool readBool()
        {
            return (buf[pos++] & 0xFF) > 0;
        }


        public void Send()
        {
            buf[0] = (byte)pos;
            buf[1] = (byte)(pos >> 8);

            socket.Send(buf, pos, SocketFlags.None);
        }

    }
}
