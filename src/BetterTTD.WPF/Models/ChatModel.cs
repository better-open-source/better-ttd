using System;
using BetterTTD.Domain.Enums;
using CSharpFunctionalExtensions;

namespace BetterTTD.WPF.Models
{
    public class ChatModel : BaseModel
    {
        public ChatModel(DestType destType, Maybe<ClientModel> maybeClient, string message)
        {
            DestType = destType switch
            {
                Domain.Enums.DestType.DESTTYPE_BROADCAST => "All",
                Domain.Enums.DestType.DESTTYPE_TEAM => "Team",
                Domain.Enums.DestType.DESTTYPE_CLIENT => "Personal",
                _ => throw new ArgumentOutOfRangeException(nameof(destType), destType, null)
            };

            Message = message;

            maybeClient
                .Match(client =>
                {
                    ClientId = client.Id;
                    ClientName = client.Name;
                }, () => Console.WriteLine($"[{nameof(ChatModel)}] Unknown client handled."));
        }
        
        public string DestType
        {
            get => Get("Unknown");
            set => Set(value);
        }

        public long ClientId
        {
            get => Get((long) -1);
            set => Set(value);
        }

        public string ClientName
        {
            get => Get("Unknown");
            set => Set(value);
        }

        public string Message
        {
            get => Get(string.Empty);
            set => Set(value);
        }
    }
}