#nullable enable

using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using BetterTTD.App.UI.Connect.Abstractions;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Connect
{
    public partial class ConnectPresenter : ReactiveObject, IRoutableViewModel, IConnectInteractorNotifier
    {
        private readonly IConnectInteractor _interactor;
        private readonly IConnectRouter _router;
        
        private bool _isLoading;
        
        public string UrlPathSegment => "/connect";
        
        public IScreen HostScreen { get; }

        public ConnectPresenter(IScreen? screen)
        {
            var canConnect = this
                .WhenAnyValue(
                    x => x.Host,
                    x => x.Port,
                    x => x.Password,
                    (host, port, password) =>
                        !(string.IsNullOrWhiteSpace(host) ||
                          string.IsNullOrWhiteSpace(password) ||
                          port is 0) &&
                        !_isLoading);

            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            
            _router = new ConnectRouter(HostScreen);
            _interactor = new ConnectInteractor(this);
            _connect = ReactiveCommand.CreateFromTask(Connect, canConnect);

            Host = "127.0.0.1";
            Port = 3977;
            Password = "p7gvv";
        }
        
        private readonly ReactiveCommand<Unit, Unit> _connect;
        public ICommand ConnectCommand => _connect;

        private async Task Connect()
        {
            _isLoading = true;
            await _interactor.ConnectAsync(Host, Port, Password);
        }
        
        public void ConnectionFailed(string error)
        {
            _isLoading = false;
        }

        public void Connected()
        {
            _isLoading = false;
            _router.NavigateToMain();
        }
    }
}