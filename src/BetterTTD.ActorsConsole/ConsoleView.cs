using System;
using BetterTTD.Actors;
using BetterTTD.Network;

namespace BetterTTD.ActorsConsole
{
    public class ConsoleView : IClientView
    {
        private readonly IClientBridge _bridge;
        private Protocol _protocol;

        public ConsoleView(ClientSystem system)
        {
            _bridge = system.CreateClientBridge(this);
        }

        public void Connect(string host, int port, string adminPassword)
        {
            _bridge.Connect(host, port, adminPassword);
        }
        
        public void OnProtocol(Protocol protocol)
        {
            _protocol = protocol;
            Console.WriteLine($"{nameof(OnProtocol)}: version #{protocol.Version}");
        }

        public void OnServerWelcome()
        {
            _bridge.SetDefaultUpdateFrequency(_protocol);
            _bridge.PollAll(_protocol);
            Console.WriteLine($"{nameof(OnServerWelcome)}");
        }
    }
}