﻿using System.Threading.Tasks;
using Akka.Actor;
using BetterTTD.Actors.ClientBridgeGroup;
using BetterTTD.Actors.ClientGroup;

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

        public IClientCommander CreateClientCommander(IClientView view)
        {
            var bridgeActor = _actorSystem.ActorOf(ClientBridgeActor.Props(view, _clientActor), nameof(ClientBridgeActor));
            return new ClientCommander(bridgeActor);
        }

        public IClientConnector CreateClientConnector(IConnectorView view)
        {
            var connectorActor = _actorSystem.ActorOf(ConnectBridgeActor.Props(_clientActor, view), nameof(ConnectBridgeActor));
            return new ClientConnector(connectorActor);
        }

        public async Task TerminateAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}