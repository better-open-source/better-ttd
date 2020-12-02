namespace BetterTTD.Coan.Enums
{
    public enum AdminUpdateFrequency
    {
        ADMIN_FREQUENCY_POLL      = 0x01,
        ADMIN_FREQUENCY_DAILY     = 0x02,
        ADMIN_FREQUENCY_WEEKLY    = 0x04,
        ADMIN_FREQUENCY_MONTHLY   = 0x08,
        ADMIN_FREQUENCY_QUARTERLY = 0x10,
        ADMIN_FREQUENCY_ANUALLY   = 0x20,
        ADMIN_FREQUENCY_AUTOMATIC = 0x40
    }
}