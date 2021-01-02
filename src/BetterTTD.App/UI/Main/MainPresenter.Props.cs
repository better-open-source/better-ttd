using BetterTTD.App.BL.Models;
using ReactiveUI;

namespace BetterTTD.App.UI.Main
{
    public partial class MainPresenter
    {
        private string _clients = "Clients: 0";
        public string Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref  _clients, value);
        }

        private string _companies = "Companies: 0";
        public string Companies
        {
            get => _companies;
            set => this.RaiseAndSetIfChanged(ref  _companies, value);
        }

        private GameModel _game;
        public GameModel Game
        {
            get => _game;
            set => this.RaiseAndSetIfChanged(ref  _game, value);
        }
    }
}