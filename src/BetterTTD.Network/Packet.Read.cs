using System.Text;

namespace BetterTTD.Network
{
    public partial class Packet
    {
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
    }
}