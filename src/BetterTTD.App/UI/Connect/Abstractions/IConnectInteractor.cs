using System.Threading.Tasks;

namespace BetterTTD.App.UI.Connect.Abstractions
{
    public interface IConnectInteractor
    {
        Task ConnectAsync(string host, int port, string password);
    }
}