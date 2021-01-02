#nullable enable

using BetterTTD.App.BL.Models;
using BetterTTD.App.UI.Main.Abstractions;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Main
{
    public partial class MainPresenter : ReactiveObject, IRoutableViewModel, IMainInteractorNotifier
    {
        private readonly IMainInteractor _interactor;

        public string UrlPathSegment => "/main";
        
        public IScreen HostScreen { get; }

        public MainPresenter(IScreen? screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _interactor = new MainInteractor(this);
        }

        public void ClientCountUpdate(int count)
        {
            Clients = $"Clients: {count}";
        }

        public void CompaniesCountUpdate(int count)
        {
            Companies = $"Companies: {count}";
        }

        public void GameUpdate(GameModel game)
        {
            Game = game;
        }
    }
}