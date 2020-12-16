using System;
using Akka.Actor;
using BetterTTD.Actors;

namespace BetterTTD.ActorsConsole
{
    internal static class Program
    {
        private static void Main()
        {
            var system = new ClientSystem("ottd-system");
            var view = new ConsoleView(system);
            view.Connect("127.0.0.1", 3977, "p7gvv");
            
            Console.Read();
        }
    }

    public sealed class ClientSystem
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _clientActor;

        public ClientSystem(string systemName)
        {
            _actorSystem = ActorSystem.Create(systemName);
            _clientActor = _actorSystem.ActorOf(ClientActor.Props(), nameof(ClientActor));
        }

        public IClientBridge CreateClientBridge(IClientView clientView)
        {
            var bridgeActor = _actorSystem.ActorOf(BridgeActor.Props(clientView, _clientActor), nameof(BridgeActor));
            return new ClientBridge(bridgeActor);
        }
    }
}