using Avalonia;
using Avalonia.Markup.Xaml;
using BetterTTD.App.UI.Chat;
using BetterTTD.App.UI.Connect;
using BetterTTD.App.UI.Console;
using BetterTTD.App.UI.Main;
using BetterTTD.App.UI.Root;
using ReactiveUI;
using Splat;

namespace BetterTTD.App
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            Locator.CurrentMutable.RegisterConstant<IScreen>(new RootPresenter());
            Locator.CurrentMutable.Register<IViewFor<ConnectPresenter>>(() => new ConnectView());
            Locator.CurrentMutable.Register<IViewFor<MainPresenter>>(() => new MainView());
            Locator.CurrentMutable.Register<IViewFor<ChatPresenter>>(() => new ChatView());
            Locator.CurrentMutable.Register<IViewFor<ConsolePresenter>>(() => new ConsoleView());

            new RootWindow {DataContext = Locator.Current.GetService<IScreen>()}.Show();
            base.OnFrameworkInitializationCompleted();
        }
    }
}
