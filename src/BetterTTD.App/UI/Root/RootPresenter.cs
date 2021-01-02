using System.Reactive;
using System.Windows.Input;
using BetterTTD.App.UI.Root.Abstractions;
using ReactiveUI;

namespace BetterTTD.App.UI.Root
{
    public class RootPresenter : ReactiveObject, IScreen
    {
        private RoutingState _routingState = new RoutingState();

        public RootPresenter()
        {
            IRootRouter router = new RootRouter(this);
            _connect = ReactiveCommand.Create(() => router.NavigateToConnect());
        }
        
        public RoutingState Router
        {
            get => _routingState;
            set => this.RaiseAndSetIfChanged(ref _routingState, value);
        }
        
        private readonly ReactiveCommand<Unit, Unit> _connect;
        public ICommand ConnectCommand => _connect;
    }
}