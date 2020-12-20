using BetterTTD.WPF.ViewModels;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace BetterTTD.WPF
{
    public class ViewModelLocator
    {
        private readonly SimpleIoc _ioc;
        
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            _ioc = SimpleIoc.Default;
            
            _ioc.Register<MainWindowViewModel>();
            _ioc.Register<ConnectViewModel>();
            _ioc.Register<HomeViewModel>();
        }
        
        public MainWindowViewModel MainWindowViewModel => _ioc.GetInstance<MainWindowViewModel>();
        public ConnectViewModel ConnectViewModel => _ioc.GetInstance<ConnectViewModel>();
        public HomeViewModel HomeViewModel => _ioc.GetInstance<HomeViewModel>();
    }
}
