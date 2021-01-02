using System.Reactive;
using BetterTTD.App.BL.Messages;
using BetterTTD.App.UI.Chat.Abstractions;
using ReactiveUI;

namespace BetterTTD.App.UI.Chat
{
    public class ChatInteractor : IChatInteractor 
    {
        public ChatInteractor(IChatInteractorNotifier notifier)
        {
            MessageBus.Current
                .Listen<ChatUpdateMessage>()
                .Subscribe(Observer.Create<ChatUpdateMessage>(msg => notifier.ChatMessageUpdate(msg.Message)));
        }
    }
}