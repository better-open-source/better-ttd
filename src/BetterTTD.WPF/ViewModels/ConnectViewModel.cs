using System;
using System.Threading.Tasks;
using BetterTTD.Actors.ClientGroup;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace BetterTTD.WPF.ViewModels
{
    public class ConnectViewModel : BaseViewModel, IConnectorView
    {
        private ClientSystem _system;
        private IClientConnector _connector;
        private bool _isRunning;

        public RelayCommand LoginCommand => new RelayCommand(LoginCommandHandler);

        public string Host
        {
            get => Get("127.0.0.1");
            set => Set(value);
        }

        public int Port
        {
            get => Get(3977);
            set => Set(value);
        }

        public string Password
        {
            get => Get(string.Empty);
            set => Set(value);
        }

        public string Error
        {
            get => Get("Error happened");
            set => Set(value);
        }

        public bool ShowError
        {
            get => Get(false);
            set => Set(value);
        }

        private async void LoginCommandHandler()
        {
            if (_isRunning)
                return;
            
            _isRunning = true;
            
            await Task.Run(() =>
            {
                _system = new ClientSystem("ottd-system");

                SimpleIoc.Default.Register(() => _system);
                SimpleIoc.Default.Unregister<HomeViewModel>();
                SimpleIoc.Default.Register<HomeViewModel>();
                SimpleIoc.Default.GetInstance<HomeViewModel>();

                Task.Delay(TimeSpan.FromMilliseconds(50));
                
                _connector = _system.CreateClientConnector(this);
                _connector.Connect(Host, Port, Password);
            });
        }

        public async void ConnectResponse(bool connected, string error)
        {
            try
            {
                _isRunning = false;
                
                if (connected)
                {
                    Messenger.Default.Send(new ShowHomeMessage());
                }
                else
                {
                    SimpleIoc.Default.Unregister<ClientSystem>();
                    await _system.TerminateAsync();
                }
                
                Error = error;
                ShowError = !connected;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}