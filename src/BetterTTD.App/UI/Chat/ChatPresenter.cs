using Avalonia.Threading;
using BetterTTD.App.BL.Models;
using BetterTTD.App.UI.Chat.Abstractions;
using DynamicData.Binding;
using ReactiveUI;

namespace BetterTTD.App.UI.Chat
{
    public class ChatPresenter : ReactiveObject, IChatInteractorNotifier
    {
        private readonly IChatInteractor _interactor;
        
        public ChatPresenter()
        {
            _interactor = new ChatInteractor(this);
        }
        
        private IObservableCollection<ChatModel> _chatMessages = new ObservableCollectionExtended<ChatModel>();
        public IObservableCollection<ChatModel> ChatMessages
        {
            get => _chatMessages;
            set => this.RaiseAndSetIfChanged(ref _chatMessages, value);
        }

        public void ChatMessageUpdate(ChatModel message)
        {
            Dispatcher.UIThread.Post(() => ChatMessages.Add(message));
        }
    }
}