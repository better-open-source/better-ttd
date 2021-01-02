using BetterTTD.App.BL.Models;

namespace BetterTTD.App.BL.Messages
{
    public class ChatUpdateMessage
    {
        public ChatUpdateMessage(ChatModel message)
        {
            Message = message;
        }

        public ChatModel Message { get; }
    }
}