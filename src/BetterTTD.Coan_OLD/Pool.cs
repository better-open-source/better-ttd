using BetterTTD.Coan_OLD.Pools;

namespace BetterTTD.Coan_OLD
{
    public class Pool
    {
        public ClientPool ClientPool { get; } = new();
        public CompanyPool CompanyPool { get; } = new();
    }
}