using BetterTTD.App.BL.Models;

namespace BetterTTD.App.UI.Main.Abstractions
{
    public interface IMainInteractorNotifier
    {
        void ClientCountUpdate(int count);
        void CompaniesCountUpdate(int count);
        void GameUpdate(GameModel game);
    }
}