using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace BetterTTD.App.UI.Console
{
    public class ConsoleView : ReactiveUserControl<ConsolePresenter>
    {
        public ConsoleView()
        {
            this.WhenActivated(disposable => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}