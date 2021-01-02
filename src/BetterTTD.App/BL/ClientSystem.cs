using System.Threading.Tasks;
using Akka.Actor;
using BetterTTD.Actors.Abstractions;
using BetterTTD.Actors.ClientBridgeGroup;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.App.BL.Bridges;

namespace BetterTTD.App.BL
{
    public sealed class ClientSystem
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _clientActor;

        public ClientSystem(string systemName)
        {
            _actorSystem = ActorSystem.Create(systemName);
            _clientActor = _actorSystem.ActorOf(ClientActor.Props(), nameof(ClientActor));
        }

        public IClientBridge CreateClientBridge(IClientView view)
        {
            var bridgeActor = _actorSystem.ActorOf(ClientBridgeActor.Props(view, _clientActor), nameof(ClientBridgeActor));
            return new ClientBridge(bridgeActor);
        }

        public IConnectBridge CreateConnectBridge(IConnectView view)
        {
            var connectActor = _actorSystem.ActorOf(ConnectBridgeActor.Props(view, _clientActor), nameof(ConnectBridgeActor));
            return new ConnectBridge(connectActor);
        }

        public async Task TerminateAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}