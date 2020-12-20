using Akka.Actor;
using BetterTTD.Network;

namespace BetterTTD.Actors.ClientGroup
{
    public interface IClientCommander
    {
        void SetDefaultUpdateFrequency(Protocol protocol);
        void PollAll(Protocol protocol);
    }

    public class ClientCommander : IClientCommander
    {
        private readonly IActorRef _bridgeActor;

        public ClientCommander(IActorRef bridgeActor)
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