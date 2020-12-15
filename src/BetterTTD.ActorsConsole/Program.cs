using System;
using Akka.Actor;
using BetterTTD.Actors;

namespace BetterTTD.ActorsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("actor-system");
            
            var serverRef = system.ActorOf(Props.Create<ServerActor>(), "ottd-server");

            Console.Read();
        }
    }
}