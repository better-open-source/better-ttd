using Akka.Actor;
using Akka.Event;
using BetterTTD.Actors.Abstractions;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.Actors.ClientGroup.ReceiverGroup;

namespace BetterTTD.Actors.ClientBridgeGroup
{
    public class ConnectBridgeActor : ReceiveActor
    {
        private readonly IConnectView _view;
        private readonly IActorRef _clientActor;

        public ConnectBridgeActor(IActorRef clientActor, IConnectView view)
        {
            _clientActor = clientActor;
            _view = view;

            Become(Active);

            Context.GetLogger().Info("Initialized");
        }

        public static Props Props(IConnectView view, IActorRef clientActor)
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