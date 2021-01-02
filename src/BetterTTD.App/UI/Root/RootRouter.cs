using BetterTTD.App.UI.Connect;
using BetterTTD.App.UI.Root.Abstractions;
using ReactiveUI;

namespace BetterTTD.App.UI.Root
{
    public class RootRouter : IRootRouter
    {
        private readonly RoutingState _router;
        private readonly IScreen _screen;

        public RootRouter(IScreen screen)
        {
            _router = screen.Router;
            _screen = screen;
        }

        public void NavigateToConnect()
        {
            _router.NavigateAndReset.Execute(new ConnectPresenter(_screen));
        }
    }
}