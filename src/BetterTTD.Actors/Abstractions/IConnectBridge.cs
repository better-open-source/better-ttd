#nullable enable
namespace BetterTTD.Actors.Abstractions
{
    public interface IConnectBridge
    {
        void Connect(string host, int port, string password);
    }
}