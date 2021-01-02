using System.Collections.Generic;
using System.Linq;
using Akka.Util.Internal;
using BetterTTD.Actors.Abstractions;
using BetterTTD.Actors.ClientGroup.ReceiverGroup.DispatcherGroup;
using BetterTTD.App.BL;
using BetterTTD.App.BL.Messages;
using BetterTTD.App.BL.Models;
using BetterTTD.App.UI.Main.Abstractions;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Main
{
    public class MainInteractor : IMainInteractor, IClientView
    {
        private readonly IMainInteractorNotifier _notifier;
        private readonly IClientBridge _bridge;

        private readonly Dictionary<int, string> _commands;
        private Protocol? _protocol;
        private Game? _game;

        private readonly List<ClientModel> _clients = new List<ClientModel>(); 
        
        public MainInteractor(IMainInteractorNotifier notifier)
        {
            _notifier = notifier;
            _commands = new Dictionary<int, string>();
            _bridge = Locator.Current.GetService<ClientSystem>().CreateClientBridge(this);
        }

        public void OnProtocol(Protocol protocol)
        {
            _protocol = protocol;
        }

        public void OnServerWelcome(Game game)
        {
            _game = game;

            _bridge.SetDefaultUpdateFrequency(_protocol);
            _bridge.PollAll(_protocol);
            
            _notifier.GameUpdate(new GameModel(_game));
        }

        public void OnServerCmdNames(Dictionary<int, string> cmdNames)
        {
            cmdNames
                .Where(cmd => !_commands.Keys.Contains(cmd.Key))
                .ForEach(kv => _commands.Add(kv.Key, kv.Value));
        }

        public void OnServerConsole(string origin, string message)
        {
            var msg = new OnServerConsoleMessage(origin, message);
            MessageBus.Current.SendMessage(msg);
        }

        public void OnServerClientInfo(Client client)
        {
            ClientModel
                .Create(client)
                .Match(
                    model =>
                    {
                        if (_clients.Contains(model)) return;
                        _clients.Add(model);
                        _notifier.ClientCountUpdate(_clients.Count);
                    },
                    System.Console.WriteLine);
        }

        public void OnServerChat(NetworkAction action, DestType dest, long clientId, string message, long data)
        {
            Maybe<ClientModel> maybeClient = _clients.FirstOrDefault(cl => cl.Id == clientId);
            var model = new ChatModel(dest, maybeClient, message);
            
            MessageBus.Current.SendMessage(new ChatUpdateMessage(model));
        }

        public void OnServerClientUpdate(long clientId, int companyId, string name)
        {
        }

        public void OnServerClientQuit(long clientId)
        {
            Maybe<ClientModel>
                .From(_clients.FirstOrDefault(cl => cl.Id == clientId))
                .Match(
                    client =>
                    {
                        _clients.Remove(client);
                        _notifier.ClientCountUpdate(_clients.Count);
                    },
                    () => System.Console.WriteLine($"Client with ID #{clientId} not found."));
        }

        public void OnServerClientError(long clientId, NetworkErrorCode errorCode)
        {
        }

        public void OnServerCompanyStats(int companyId, Dictionary<VehicleType, int> vehicles, Dictionary<VehicleType, int> stations)
        {
        }

        public void OnServerCompanyRemove(int companyId, AdminCompanyRemoveReason removeReason)
        {
        }

        public void OnServerDate(GameDate date)
        {
            if (_game is null) 
                return;

            _game.Map.CurrentDate = date;
            _notifier.GameUpdate(new GameModel(_game));
        }
    }
}