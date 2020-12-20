using System.Collections.Generic;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;

namespace BetterTTD.Actors.ClientGroup.ReceiverGroup.DispatcherGroup
{
    public class ReceivedBufMessage
    {
        public Packet Packet { get; }

        public ReceivedBufMessage(Packet packet)
        {
            Packet = packet;
        }
    }

    public class OnServerCmdNamesMessage
    {
        public Dictionary<int, string> CmdNames { get; }

        public OnServerCmdNamesMessage(Dictionary<int, string> cmdNames)
        {
            CmdNames = cmdNames;
        }
    }

    public class OnServerWelcomeMessage
    {
        public Game Game { get; }

        public OnServerWelcomeMessage(Game game)
        {
            Game = game;
        }
    }

    public class OnProtocolMessage
    {
        public Protocol Protocol { get; }

        public OnProtocolMessage(Protocol protocol)
        {
            Protocol = protocol;
        }
    }

    public class OnServerConsoleMessage
    {
        public string Origin { get; }
        public string Message { get; }

        public OnServerConsoleMessage(string origin, string message)
        {
            Origin = origin;
            Message = message;
        }
    }

    public class OnServerClientInfoMessage
    {
        public Client Client { get; }

        public OnServerClientInfoMessage(Client client)
        {
            Client = client;
        }
    }

    public class OnServerChatMessage
    {
        public NetworkAction Action { get; }
        public DestType Dest { get; }
        public long ClientId { get; }
        public string Message { get; }
        public long Data { get; }

        public OnServerChatMessage(
            NetworkAction action,
            DestType dest,
            long clientId,
            string message,
            long data)
        {
            Action = action;
            Dest = dest;
            ClientId = clientId;
            Message = message;
            Data = data;
        }
    }

    public class OnServerClientUpdateMessage
    {
        public long ClientId { get; }
        public string Name { get; }
        public int CompanyId { get; }

        public OnServerClientUpdateMessage(
            long clientId,
            string name,
            int companyId)
        {
            ClientId = clientId;
            Name = name;
            CompanyId = companyId;
        }
    }

    public class OnServerClientQuitMessage
    {
        public long ClientId { get; }

        public OnServerClientQuitMessage(long clientId)
        {
            ClientId = clientId;
        }
    }

    public class OnServerClientErrorMessage
    {
        public long ClientId { get; }
        public NetworkErrorCode ErrorCode { get; }

        public OnServerClientErrorMessage(long clientId, NetworkErrorCode errorCode)
        {
            ClientId = clientId;
            ErrorCode = errorCode;
        }
    }

    public class OnServerCompanyStatsMessage
    {
        public int CompanyId { get; }
        public Dictionary<VehicleType, int> Vehicles { get; }
        public Dictionary<VehicleType, int> Stations { get; }

        public OnServerCompanyStatsMessage(
            int companyId,
            Dictionary<VehicleType, int> vehicles,
            Dictionary<VehicleType, int> stations)
        {
            CompanyId = companyId;
            Vehicles = vehicles;
            Stations = stations;
        }
    }

    public class OnServerCompanyRemoveMessage
    {
        public int CompanyId { get; }
        public AdminCompanyRemoveReason RemoveReason { get; }

        public OnServerCompanyRemoveMessage(int companyId, AdminCompanyRemoveReason removeReason)
        {
            CompanyId = companyId;
            RemoveReason = removeReason;
        }
    }
}