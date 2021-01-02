using BetterTTD.App.BL.Models;

namespace BetterTTD.App.UI.Chat.Abstractions
{
    public interface IChatInteractorNotifier
    {
        void ChatMessageUpdate(ChatModel message);
    }
}