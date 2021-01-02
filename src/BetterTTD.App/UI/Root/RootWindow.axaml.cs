using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace BetterTTD.App.UI.Root
{
    public sealed class RootWindow : ReactiveWindow<RootPresenter>
    {
        public RootWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
