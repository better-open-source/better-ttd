using System;
using System.Net.Sockets;
using Akka.Actor;
using Akka.Event;
using BetterTTD.Actors.ClientGroup.ReceiverGroup.DispatcherGroup;
using BetterTTD.Network;
using CSharpFunctionalExtensions;

namespace BetterTTD.Actors.ClientGroup.ReceiverGroup
{
    public class ReceiverActor : ReceiveActor, IWithTimers
    {
        private readonly Socket _socket;
        private readonly ActorSelection _connectorBridge;
        private readonly ILoggingAdapter _log;
        private readonly IActorRef _dispatcher;

        public ITimerScheduler Timers { get; set; }

        public ReceiverActor(Socket socket)
        {
            _socket = socket;
            _log = Context.GetLogger();
            _connectorBridge = Context.ActorSelection("akka://ottd-system/user/ConnectBridgeActor");
            _dispatcher = Context.ActorOf(DispatcherActor.Props(), nameof(DispatcherActor));

            Receive<ReceiveBufMessage>(ReceiveBufMessageHandler);
            
            _log.Info("Initialized");
        }

        public static Props Props(Socket socket)
        {
            return Akka.Actor.Props.Create(() => new ReceiverActor(socket));
        }

        protected override void PreStart()
        {
            _log.Info($"PreStart for {nameof(ReceiverActor)}");

            Timers.StartPeriodicTimer(
                nameof(ReceiverActor), 
                new ReceiveBufMessage(), 
                TimeSpan.FromMilliseconds(1));
        }
        
        private void ReceiveBufMessageHandler(ReceiveBufMessage _)
        {
            var (isSuccess, _, packet, error) = Packet.Create(_socket);
            if (isSuccess)
            {
                _dispatcher.Tell(new ReceivedBufMessage(packet));
                _log.Info($"Received Packet Type: {packet.GetPacketType()}");
            }
            else
            {
                _log.Error($"Received Packet Error: {error}");
                
                _connectorBridge.Tell(new ReceiveSocketErrorMessage(error));
                
                Timers.CancelAll();
            }
        }
    }
}