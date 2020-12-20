using System.Collections.Generic;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterTTD.Actors.ClientGroup
{
    public interface IClientView
    {
        void OnProtocol(Protocol protocol);
        void OnServerWelcome(Game game);
        void OnServerCmdNames(Dictionary<int,string> cmdNames);
        void OnServerConsole(string origin, string message);
        void OnServerClientInfo(Client client);
        void OnServerChat(NetworkAction action, DestType dest, long clientId, string message, long data);
        void OnServerClientUpdate(long clientId, int companyId, string name);
        void OnServerClientQuit(long clientId);
        void OnServerClientError(long clientId, NetworkErrorCode errorCode);
        void OnServerCompanyStats(int companyId, Dictionary<VehicleType,int> vehicles, Dictionary<VehicleType,int> stations);
        void OnServerCompanyRemove(int companyId, AdminCompanyRemoveReason removeReason);
    }
}