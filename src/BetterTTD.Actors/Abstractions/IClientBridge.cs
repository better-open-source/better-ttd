using BetterTTD.Network;

namespace BetterTTD.Actors.Abstractions
{
    public interface IClientBridge
    {
        void SetDefaultUpdateFrequency(Protocol protocol);
        void PollAll(Protocol protocol);
    }
}