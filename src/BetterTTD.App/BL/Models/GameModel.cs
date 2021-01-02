using BetterTTD.Domain.Entities;
using ReactiveUI;

namespace BetterTTD.App.BL.Models
{
    public sealed class GameModel : ReactiveObject
    {
        private string? _name;
        private string? _gameVersion;
        private int _versionProtocol;
        private bool _dedicated;
        private MapModel _map;

        public GameModel(Game game)
        {
            Name = game.Name;
            GameVersion = game.GameVersion;
            VersionProtocol = game.VersionProtocol;
            Dedicated = game.Dedicated;
            Map = new MapModel(game.Map);
        }
        
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string? GameVersion
        {
            get => _gameVersion;
            set => this.RaiseAndSetIfChanged(ref _gameVersion, value);
        }

        public int VersionProtocol
        {
            get => _versionProtocol;
            set => this.RaiseAndSetIfChanged(ref _versionProtocol, value);
        }

        public bool Dedicated
        {
            get => _dedicated;
            set => this.RaiseAndSetIfChanged(ref _dedicated, value);
        }

        public MapModel Map
        {
            get => _map;
            set => this.RaiseAndSetIfChanged(ref _map, value);
        }
    }
}