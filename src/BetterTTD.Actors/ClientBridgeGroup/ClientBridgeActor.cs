using Akka.Actor;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.Actors.ClientGroup.ReceiverGroup.DispatcherGroup;

namespace BetterTTD.Actors.ClientBridgeGroup
{
    public class ClientBridgeActor : ReceiveActor
    {
        private readonly IClientView _view;
        private readonly IActorRef _clientActor;

        public ClientBridgeActor(IClientView view, IActorRef clientActor)
        {
            _clientActor = clientActor;
            _view = view;
            
            Become(Active);
        }

        public static Props Props(IClientView view, IActorRef clientActor)
        {
            return Akka.Actor.Props.Create(() => new ClientBridgeActor(view, clientActor));
        }

        private void Active()
        {
            Receive<OnProtocolMessage>(msg => _view.OnProtocol(msg.Protocol));
            Receive<OnServerWelcomeMessage>(msg => _view.OnServerWelcome(msg.Game));
            Receive<OnServerCmdNamesMessage>(msg => _view.OnServerCmdNames(msg.CmdNames));
            Receive<OnServerConsoleMessage>(msg => _view.OnServerConsole(msg.Origin, msg.Message));
            Receive<OnServerClientInfoMessage>(msg => _view.OnServerClientInfo(msg.Client));
            Receive<OnServerChatMessage>(msg => _view.OnServerChat(msg.Action, msg.Dest, msg.ClientId, msg.Message, msg.Data));
            Receive<OnServerClientUpdateMessage>(msg => _view.OnServerClientUpdate(msg.ClientId, msg.CompanyId, msg.Name));
            Receive<OnServerClientQuitMessage>(msg => _view.OnServerClientQuit(msg.ClientId));
            Receive<OnServerClientErrorMessage>(msg => _view.OnServerClientError(msg.ClientId, msg.ErrorCode));
            Receive<OnServerCompanyStatsMessage>(msg => _view.OnServerCompanyStats(msg.CompanyId, msg.Vehicles, msg.Stations));
            Receive<OnServerCompanyRemoveMessage>(msg => _view.OnServerCompanyRemove(msg.CompanyId, msg.RemoveReason));
            
            Receive<SetDefaultUpdateFrequencyMessage>(_clientActor.Tell);
            Receive<PollAllMessage>(_clientActor.Tell);
        }
    }
}