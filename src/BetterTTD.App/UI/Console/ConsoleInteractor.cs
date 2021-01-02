using System.Reactive;
using BetterTTD.Actors.ClientGroup.ReceiverGroup.DispatcherGroup;
using BetterTTD.App.BL.Models;
using BetterTTD.App.UI.Console.Abstractions;
using ReactiveUI;

namespace BetterTTD.App.UI.Console
{
    public class ConsoleInteractor : IConsoleInteractor
    {
        public ConsoleInteractor(IConsoleInteractorNotifier notifier)
        {
            MessageBus.Current
                .Listen<OnServerConsoleMessage>()
                .Subscribe(Observer.Create<OnServerConsoleMessage>(msg =>
                {
                    var update = new ConsoleModel(msg.Origin, msg.Message);
                    notifier.OnConsoleUpdate(update);
                }));
        }
    }
}