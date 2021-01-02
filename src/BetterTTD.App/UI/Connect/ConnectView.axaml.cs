using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace BetterTTD.App.UI.Connect
{
    public sealed class ConnectView : ReactiveUserControl<ConnectPresenter>
    {
        public ConnectView()
        {
            this.WhenActivated(disposable => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}