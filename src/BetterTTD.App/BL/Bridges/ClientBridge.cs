using Akka.Actor;
using BetterTTD.Actors.Abstractions;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.Network;

namespace BetterTTD.App.BL.Bridges
{
    public class ClientBridge : IClientBridge
    {
        private readonly IActorRef _bridgeActor;

        public ClientBridge (IActorRef bridgeActor)
        {
            _bridgeActor = bridgeActor;
        }

        public void SetDefaultUpdateFrequency(Protocol protocol)
        {
            _bridgeActor.Tell(new SetDefaultUpdateFrequencyMessage(protocol));
        }

        public void PollAll(Protocol protocol)
        {
            _bridgeActor.Tell(new PollAllMessage(protocol));
        }
    }
}