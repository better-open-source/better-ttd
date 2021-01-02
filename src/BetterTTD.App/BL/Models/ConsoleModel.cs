using ReactiveUI;

namespace BetterTTD.App.BL.Models
{
    public class ConsoleModel : ReactiveObject
    {
        private string _origin = "Unknown";
        private string _message = string.Empty;

        public ConsoleModel(string origin, string message)
        {
            Origin = origin;
            Message = message;
        }

        public string Origin
        {
            get => _origin;
            set => this.RaiseAndSetIfChanged(ref _origin, value);
        }

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
    }
}