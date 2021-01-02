using System.Threading.Tasks;

namespace BetterTTD.Actors.Abstractions
{
    public interface IConnectView
    {
        Task ConnectResponse(bool connected, string? error = null);
    }
}