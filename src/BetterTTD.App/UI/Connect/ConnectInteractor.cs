#nullable enable

using System.Threading.Tasks;
using BetterTTD.Actors.Abstractions;
using BetterTTD.App.BL;
using BetterTTD.App.UI.Connect.Abstractions;
using BetterTTD.App.UI.Main;
using Splat;

namespace BetterTTD.App.UI.Connect
{
    public class ConnectInteractor : IConnectInteractor, IConnectView
    {
        private readonly IConnectInteractorNotifier _notifier;
        
        private ClientSystem? _system;
        
        public ConnectInteractor(IConnectInteractorNotifier notifier)
        {
            _notifier = notifier;
        }

        public async Task ConnectAsync(string host, int port, string password)
        {
            _system = new ClientSystem("ottd-system");
            Locator.CurrentMutable.RegisterConstant(_system);
            Locator.CurrentMutable.UnregisterCurrent<MainPresenter>();
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainPresenter(null));
            Locator.Current.GetService<MainPresenter>();

            await Task.Delay(100);

            var bridge = _system.CreateConnectBridge(this);
            bridge.Connect(host, port, password);
        }

        public async Task ConnectResponse(bool connected, string? error = null)
        {
            if (connected && _system != null)
            {
                _notifier.Connected();
            }
            else
            {
                await _system?.TerminateAsync()!;

                Locator.CurrentMutable.UnregisterAll<ClientSystem>();
                
                _notifier.ConnectionFailed(error ?? "Error occurred!");
            }
        }
    }
}