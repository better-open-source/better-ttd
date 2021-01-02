using BetterTTD.Domain.Entities;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace BetterTTD.App.BL.Models
{
    public class ClientModel : ReactiveObject
    {
        private long _id = -1;
        private string _name = "Unknown";
        private CompanyModel? _company;

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
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public CompanyModel? Company
        {
            get => _company;
            set => this.RaiseAndSetIfChanged(ref _company, value);
        }
    }
}