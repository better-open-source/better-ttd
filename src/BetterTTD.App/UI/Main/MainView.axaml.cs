using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace BetterTTD.App.UI.Main
{
    public sealed class MainView : ReactiveUserControl<MainPresenter>
    {
        public MainView()
        {
            this.WhenActivated(disposable => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}