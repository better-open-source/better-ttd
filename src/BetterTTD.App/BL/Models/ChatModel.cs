using System;
using BetterTTD.Domain.Enums;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace BetterTTD.App.BL.Models
{
    public class ChatModel : ReactiveObject
    {
        private string _destType = "Unknown";
        private long _clientId = -1;
        private string _clientName = "Unknown";
        private string _message = string.Empty;

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
            get => _destType;
            set => this.RaiseAndSetIfChanged(ref _destType, value);
        }

        public long ClientId
        {
            get => _clientId;
            set => this.RaiseAndSetIfChanged(ref _clientId, value);
        }

        public string ClientName
        {
            get => _clientName;
            set => this.RaiseAndSetIfChanged(ref _clientName, value);
        }

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
    }
}