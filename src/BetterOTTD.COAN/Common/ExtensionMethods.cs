using System.Text;
using BetterTTD.Domain.Enums;

namespace BetterOTTD.COAN.Common
{
    public static class ExtensionMethods
    {
        public static string getDispatchName(this PacketType packet)
        {
            var name = packet.ToString().Replace("ADMIN_PACKET_", "").ToLower();
            var result = (int)packet < 100 ? new("send") : new StringBuilder("receive");

            foreach (var part in name.Split('_'))
            {
                result.Append(part.Substring(0, 1).ToUpper());
                result.Append(part.Substring(1));
            }

            return result.ToString();
        }
    }
}
