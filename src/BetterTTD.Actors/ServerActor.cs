using System;
using Akka.Actor;
using Akka.Event;

namespace BetterTTD.Actors
{
    public record Print;
    
    public class ServerActor : ReceiveActor, IWithTimers
    {
        public ITimerScheduler Timers { get; set; }

        private int _count = 0;
        private readonly ILoggingAdapter _log = Context.GetLogger();
        
        public ServerActor()
        {
            Receive<int>(i => _count += i);
            Receive<Print>(_ => _log.Info("Current count is [{0}]", _count));
            
            Console.WriteLine("Start Server");
        }

        protected override void PreStart()
        {
            Timers.StartPeriodicTimer("print", new Print(), TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(5));
            Timers.StartPeriodicTimer("add", 1, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(20));
        }
    }
}