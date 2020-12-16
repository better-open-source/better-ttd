﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Akka.Actor;
using Akka.Event;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Actors
{
    public sealed class ClientActor : ReceiveActor
    {
        private Socket _socket;
        private IActorRef _sender;
        private readonly ILoggingAdapter _log;
        
        public ClientActor()
        {
            _log = Context.GetLogger();
            
            Receive<AdminConnectMessage>(ConnectMessageHandler);
            Receive<SetDefaultUpdateFrequencyMessage>(SetDefaultUpdateFrequencyMessageHandler);
            Receive<PollAllMessage>(PollAllMessageHandler);
            
            _log.Info("Initialized");
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ClientActor());
        }
        
        private void ConnectMessageHandler(AdminConnectMessage msg)
        {
            var (host, port, password) = msg;

            _log.Info($"Handled {nameof(AdminConnectMessage)}. Host:{host}; Port:{port}; Password:{password}");
            
            if (_socket?.Connected ?? false)
                return;

            _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            _socket.Connect(host, port);
            
            _ = Context.ActorOf(ReceiverActor.Props(_socket), nameof(ReceiverActor));
            _sender = Context.ActorOf(SenderActor.Props(_socket), nameof(SenderActor));
            
            _sender.Tell(new SendAdminJoinMessage(password, "BetterTTD", "1.0"));
        }
        
        private void SetDefaultUpdateFrequencyMessageHandler(SetDefaultUpdateFrequencyMessage msg)
        {
            var dict = new Dictionary<AdminUpdateType, AdminUpdateFrequency>
            {
                {AdminUpdateType.ADMIN_UPDATE_CONSOLE, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC},
                {AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC},
                {AdminUpdateType.ADMIN_UPDATE_CHAT, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC},
                {AdminUpdateType.ADMIN_UPDATE_COMPANY_ECONOMY, AdminUpdateFrequency.ADMIN_FREQUENCY_WEEKLY},
                {AdminUpdateType.ADMIN_UPDATE_COMPANY_STATS, AdminUpdateFrequency.ADMIN_FREQUENCY_WEEKLY},
                {AdminUpdateType.ADMIN_UPDATE_CMD_LOGGING, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC},
                {AdminUpdateType.ADMIN_UPDATE_GAMESCRIPT, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC}
            };

            dict.Where(x => !msg.Protocol.IsSupported(x.Key, x.Value))
                .Select(x => $"Protocol is not supported for: {x.Key} with {x.Value}")
                .ToList()
                .ForEach(error => _log.Error(error));
            
            dict
                .Where(x => msg.Protocol.IsSupported(x.Key, x.Value))
                .Select(x => new SendAdminUpdateFrequencyMessage(x.Key, x.Value))
                .ToList()
                .ForEach(_sender.Tell);
        }
       
        private void PollAllMessageHandler(PollAllMessage msg)
        {
            var pollMessages = new List<SendAdminPollMessage>
            {
                new(AdminUpdateType.ADMIN_UPDATE_CMD_NAMES),
                new(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, long.MaxValue),
                new(AdminUpdateType.ADMIN_UPDATE_COMPANY_INFO, long.MaxValue),
                new(AdminUpdateType.ADMIN_UPDATE_COMPANY_STATS),
                new(AdminUpdateType.ADMIN_UPDATE_COMPANY_ECONOMY),
                new(AdminUpdateType.ADMIN_UPDATE_DATE)
            };
            
            pollMessages
                .Where(p => !msg.Protocol.IsSupported(p.Type, AdminUpdateFrequency.ADMIN_FREQUENCY_POLL))
                .Select(p => $"Protocol is not supported for: {p.Type} and {AdminUpdateFrequency.ADMIN_FREQUENCY_POLL}")
                .ToList()
                .ForEach(error => _log.Error(error));
            
            pollMessages
                .Where(p => msg.Protocol.IsSupported(p.Type, AdminUpdateFrequency.ADMIN_FREQUENCY_POLL))
                .ToList()
                .ForEach(_sender.Tell);
        }
    }
}