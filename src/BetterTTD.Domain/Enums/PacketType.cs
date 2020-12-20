using System.Text;

namespace BetterTTD.Domain.Enums
{
    public enum PacketType
    {
        ADMIN_PACKET_ADMIN_JOIN              = 0,    /// The admin announces and authenticates itself to the server.
        ADMIN_PACKET_ADMIN_QUIT              = 1,    /// The admin tells the server that it is quitting.
        ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY  = 2,    /// The admin tells the server the update frequency of a particular piece of information.
        ADMIN_PACKET_ADMIN_POLL              = 3,    /// The admin explicitly polls for a piece of information.
        ADMIN_PACKET_ADMIN_CHAT              = 4,    /// The admin sends a chat message to be distributed.
        ADMIN_PACKET_ADMIN_RCON              = 5,    /// The admin sends a remote console command.
        ADMIN_PACKET_ADMIN_GAMESCRIPT        = 6,    /// The admin sends a JSON string for the GameScript.
        ADMIN_PACKET_ADMIN_PING              = 7,    /// The admin sends a ping to the server, expecting a ping-reply (PONG) packet.

        ADMIN_PACKET_SERVER_FULL             = 100,  /// The server tells the admin it cannot accept the admin.
        ADMIN_PACKET_SERVER_BANNED           = 101,  /// The server tells the admin it is banned.
        ADMIN_PACKET_SERVER_ERROR            = 102,  /// The server tells the admin an error has occurred.
        ADMIN_PACKET_SERVER_PROTOCOL         = 103,  /// The server tells the admin its protocol version.
        ADMIN_PACKET_SERVER_WELCOME          = 104,  /// The server welcomes the admin to a game.
        ADMIN_PACKET_SERVER_NEWGAME          = 105,  /// The server tells the admin its going to start a new game.
        ADMIN_PACKET_SERVER_SHUTDOWN         = 106,  /// The server tells the admin its shutting down.

        ADMIN_PACKET_SERVER_DATE             = 107,  /// The server tells the admin what the current game date is.
        ADMIN_PACKET_SERVER_CLIENT_JOIN      = 108,  /// The server tells the admin that a client has joined.
        ADMIN_PACKET_SERVER_CLIENT_INFO      = 109,  /// The server gives the admin information about a client.
        ADMIN_PACKET_SERVER_CLIENT_UPDATE    = 110,  /// The server gives the admin an information update on a client.
        ADMIN_PACKET_SERVER_CLIENT_QUIT      = 111,  /// The server tells the admin that a client quit.
        ADMIN_PACKET_SERVER_CLIENT_ERROR     = 112,  /// The server tells the admin that a client caused an error.
        ADMIN_PACKET_SERVER_COMPANY_NEW      = 113,  /// The server tells the admin that a new company has started.
        ADMIN_PACKET_SERVER_COMPANY_INFO     = 114,  /// The server gives the admin information about a company.
        ADMIN_PACKET_SERVER_COMPANY_UPDATE   = 115,  /// The server gives the admin an information update on a company.
        ADMIN_PACKET_SERVER_COMPANY_REMOVE   = 116,  /// The server tells the admin that a company was removed.
        ADMIN_PACKET_SERVER_COMPANY_ECONOMY  = 117,  /// The server gives the admin some economy related company information.
        ADMIN_PACKET_SERVER_COMPANY_STATS    = 118,  /// The server gives the admin some statistics about a company.
        ADMIN_PACKET_SERVER_CHAT             = 119,  /// The server received a chat message and relays it.
        ADMIN_PACKET_SERVER_RCON             = 120,  /// The server's reply to a remove console command.
        ADMIN_PACKET_SERVER_CONSOLE          = 121,  /// The server gives the admin the data that got printed to its console.
        ADMIN_PACKET_SERVER_CMD_NAMES        = 122,  /// The server gives the admin names of all DoCommands.
        ADMIN_PACKET_SERVER_CMD_LOGGING      = 123,  /// The server gives the admin DoCommand information (for logging purposes only).
        ADMIN_PACKET_SERVER_GAMESCRIPT       = 124,  /// The server gives the admin information from the GameScript in JSON.
        ADMIN_PACKET_SERVER_RCON_END         = 125,  /// The server indicates that the remote console command has completed.
        ADMIN_PACKET_SERVER_PONG             = 126,  /// The server replies to a ping request from the admin.
        ADMIN_PACKET_SERVER_END              = 127,  /// For internal reference only, mark the end.

        INVALID_ADMIN_PACKET                 = 0xFF  /// An invalid marker for admin packets.
    }
    
    public static class ExtensionMethods
    {
        public static string GetDispatchName(this PacketType packet)
        {
            var name = packet.ToString().Replace("ADMIN_PACKET_", "").ToLower();
            var result = (int)packet < 100 ? new StringBuilder("send") : new StringBuilder("receive");

            foreach (var part in name.Split('_'))
            {
                result.Append(part.Substring(0, 1).ToUpper());
                result.Append(part.Substring(1));
            }

            return result.ToString();
        }
    }
}