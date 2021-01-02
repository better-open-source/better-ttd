#nullable enable

using ReactiveUI;

namespace BetterTTD.App.UI.Connect
{
    public partial class ConnectPresenter
    {
        private string? _host;
        public string Host
        {
            get => _host ?? string.Empty; 
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        private int _port;
        public int Port
        {
            get => _port; 
            set => this.RaiseAndSetIfChanged(ref _port, value);
        }

        private string? _password;
        public string Password
        {
            get => _password ?? string.Empty; 
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }
    }
}