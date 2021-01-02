using Avalonia.Threading;
using BetterTTD.App.BL.Models;
using BetterTTD.App.UI.Console.Abstractions;
using DynamicData.Binding;
using ReactiveUI;

namespace BetterTTD.App.UI.Console
{
    public class ConsolePresenter : ReactiveObject, IConsoleInteractorNotifier
    {
        private readonly IConsoleInteractor _interactor;

        public ConsolePresenter()
        {
            _interactor = new ConsoleInteractor(this);
        }

        private IObservableCollection<ConsoleModel> _consoleMessages = new ObservableCollectionExtended<ConsoleModel>();
        public IObservableCollection<ConsoleModel> ConsoleMessages
        {
            get => _consoleMessages;
            set => this.RaiseAndSetIfChanged(ref _consoleMessages, value);
        }

        public void OnConsoleUpdate(ConsoleModel update)
        {
            Dispatcher.UIThread.Post(() => ConsoleMessages.Add(update));
        }
    }
}