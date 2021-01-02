using Avalonia.Threading;
using BetterTTD.App.UI.Connect.Abstractions;
using BetterTTD.App.UI.Main;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Connect
{
    public class ConnectRouter : IConnectRouter
    {
        private readonly RoutingState _router;

        public ConnectRouter(IScreen screen)
        {
            _router = screen.Router;
        }

        public void NavigateToMain()
        {
            var presenter = Locator.Current.GetService<MainPresenter>();
            Dispatcher.UIThread.Post(() =>
            {
                _router.NavigateAndReset.Execute(presenter);
            });
        }
    }
}