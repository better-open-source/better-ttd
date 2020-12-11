namespace BetterTTD.Domain.Enums
{
    public enum NetworkAction
    {
        NETWORK_ACTION_JOIN               = 0,
        NETWORK_ACTION_LEAVE              = 1,
        NETWORK_ACTION_SERVER_MESSAGE     = 2,
        NETWORK_ACTION_CHAT               = 3,
        NETWORK_ACTION_CHAT_COMPANY       = 4,
        NETWORK_ACTION_CHAT_CLIENT        = 5,
        NETWORK_ACTION_GIVE_MONEY         = 6,
        NETWORK_ACTION_NAME_CHANGE        = 7,
        NETWORK_ACTION_COMPANY_SPECTATOR  = 8,
        NETWORK_ACTION_COMPANY_JOIN       = 9,
        NETWORK_ACTION_COMPANY_NEW        = 10
    }
}