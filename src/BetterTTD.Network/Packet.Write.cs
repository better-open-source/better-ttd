using System.Text;

namespace BetterTTD.Network
{
    public partial class Packet
    {
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
    }
}