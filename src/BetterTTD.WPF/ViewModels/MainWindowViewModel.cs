using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace BetterTTD.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public BaseViewModel CurrentViewModel
        {
            get => Get<BaseViewModel>();
            set => Set(value);
        }
        
        public MainWindowViewModel()
        {
            Messenger.Default.Register<ShowConnectMessage>(this, OnShowLoginMessageHandler);
            Messenger.Default.Register<ShowHomeMessage>(this, OnShowHomeMessageHandler);
            
            CurrentViewModel = SimpleIoc.Default.GetInstance<ConnectViewModel>();
        }

        private void OnShowHomeMessageHandler(ShowHomeMessage msg)
        {
            var vm = SimpleIoc.Default.GetInstance<HomeViewModel>();
            CurrentViewModel = vm;
        }

        private void OnShowLoginMessageHandler(ShowConnectMessage msg)
        {
            var vm = SimpleIoc.Default.GetInstance<ConnectViewModel>();
            CurrentViewModel = vm;
        }
    }

    public class ShowConnectMessage { }

    public class ShowHomeMessage { }
}