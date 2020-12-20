using BetterTTD.Domain.Entities;
using CSharpFunctionalExtensions;

namespace BetterTTD.WPF.Models
{
    public class ClientModel : BaseModel
    {
        private ClientModel(Client client)
        {
            Id = client.Id;
            Name = client.Name;
        }

        public static Result<ClientModel> Create(Maybe<Client> maybeClient) =>
            maybeClient
                .Match(
                    client => new ClientModel(client),
                    () => Result.Failure<ClientModel>($"[{nameof(ClientModel)}] Unknown client handled."));

        public long Id
        {
            get => Get((long) -1);
            set => Set(value);
        }
        
        public string Name
        {
            get => Get("Unknown");
            set => Set(value);
        }
        
        public Company Company
        {
            get => Get<Company>();
            set => Set(value);
        }
    }
}