using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace BetterTTD.App.UI.Chat
{
    public class ChatView : ReactiveUserControl<ChatPresenter>
    {
        public ChatView()
        {
            this.WhenActivated(disposable => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}