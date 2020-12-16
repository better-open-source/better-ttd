﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using BetterTTD.Domain.Entities;
using BetterTTD.Network;

namespace BetterTTD.Actors
{
    public class ClientBridge : IClientBridge
    {
        private readonly IActorRef _bridgeActor;

        public ClientBridge(IActorRef bridgeActor)
        {
            _bridgeActor = bridgeActor;
            Console.WriteLine($"[{nameof(ClientBridge)}] initialized");
        }

        public void Connect(string host, int port, string adminPassword)
        {
            _bridgeActor.Tell(new AdminConnectMessage(host, port, adminPassword));
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

    public interface IClientView
    {
        void Connect(string host, int port, string adminPassword);
        
        void OnProtocol(Protocol protocol);
        void OnServerWelcome(Game game);
        void OnServerCmdNames(Dictionary<int,string> cmdNames);
        void OnServerConsole(string origin, string message);
        void OnServerClientInfo(Client client);
    }

    public interface IClientBridge
    {
        void Connect(string host, int port, string adminPassword);
        void SetDefaultUpdateFrequency(Protocol protocol);
        void PollAll(Protocol protocol);
    }
}