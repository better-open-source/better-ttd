using Akka.Actor;
using Akka.Event;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.Actors.ClientGroup.ReceiverGroup;

namespace BetterTTD.Actors.ClientBridgeGroup
{
    public class ConnectBridgeActor : ReceiveActor
    {
        private readonly IConnectorView _view;
        private readonly IActorRef _clientActor;

        public ConnectBridgeActor(IActorRef clientActor, IConnectorView view)
        {
            _clientActor = clientActor;
            _view = view;

            Become(Active);

            Context.GetLogger().Info("Initialized");
        }

        public static Props Props(IActorRef clientActor, IConnectorView view)
        {
            return Akka.Actor.Props.Create(() => new ConnectBridgeActor(clientActor, view));
        }

        private void Active()
        {
            Receive<AdminConnectMessage>(_clientActor.Tell);

            Receive<OnAdminConnectMessage>(msg => _view.ConnectResponse(true));
            Receive<ReceiveSocketErrorMessage>(msg => _view.ConnectResponse(false, msg.Error));
        }
    }
}