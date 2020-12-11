namespace BetterTTD.Domain.Enums
{
    public enum PauseMode
    {
        PM_UNPAUSED              = 0,  /// A normal unpaused game
        PM_PAUSED_NORMAL         = 1,  /// A game normally paused
        PM_PAUSED_SAVELOAD       = 2,  /// A game paused for saving/loading
        PM_PAUSED_JOIN           = 4,  /// A game paused for 'pause_on_join'
        PM_PAUSED_ERROR          = 8,  /// A game paused because a (critical) error
        PM_PAUSED_ACTIVE_CLIENTS = 16, /// A game paused for 'min_active_clients'
        PM_PAUSED_GAME_SCRIPT    = 32, /// A game paused by a game script

        /* TODO: Pause mode bits when paused for network reasons. */
        //PMB_PAUSED_NETWORK  (PM_PAUSED_ACTIVE_CLIENTS.value | PM_PAUSED_JOIN.value);
    }
}