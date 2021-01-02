using Akka.Actor;
using BetterTTD.Actors.Abstractions;
using BetterTTD.Actors.ClientGroup;

namespace BetterTTD.App.BL.Bridges
{
    public class ConnectBridge : IConnectBridge
    {
        private readonly IActorRef _bridgeActor;

        public ConnectBridge(IActorRef bridgeActor)
        {
            _bridgeActor = bridgeActor;
        }

        public void Connect(string host, int port, string password)
        {
            _bridgeActor.Tell(new AdminConnectMessage(host, port, password));
        }
    }
}