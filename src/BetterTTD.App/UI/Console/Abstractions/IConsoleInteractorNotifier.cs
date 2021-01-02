using BetterTTD.App.BL.Models;

namespace BetterTTD.App.UI.Console.Abstractions
{
    public interface IConsoleInteractorNotifier
    {
        void OnConsoleUpdate(ConsoleModel update);
    }
}