namespace BetterTTD.App.UI.Connect.Abstractions
{
    public interface IConnectInteractorNotifier
    {
        void ConnectionFailed(string error);
        void Connected();
    }
}