using Akka.Actor;
using Akka.Event;

namespace BetterTTD.Actors
{
    public class BridgeActor : ReceiveActor
    {
        private readonly IClientView _clientView;
        private readonly IActorRef _clientActor;
        private readonly ILoggingAdapter _log;

        public BridgeActor(IClientView clientView, IActorRef clientActor)
        {
            _clientView = clientView;
            _clientActor = clientActor;
            _log = Context.GetLogger();
            
            Become(Active);

            _log.Info("Initialized");
        }

        public static Props Props(IClientView clientView, IActorRef clientActor)
        {
            return Akka.Actor.Props.Create(() => new BridgeActor(clientView, clientActor));
        }

        private void Active()
        {
            Receive<OnProtocolMessage>(msg => _clientView.OnProtocol(msg.Protocol));
            Receive<OnServerWelcomeMessage>(msg => _clientView.OnServerWelcome());
            
            Receive<AdminConnectMessage>(_clientActor.Tell);
            Receive<SetDefaultUpdateFrequencyMessage>(_clientActor.Tell);
            Receive<PollAllMessage>(_clientActor.Tell);
        }
    }
}