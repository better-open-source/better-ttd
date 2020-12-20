using System.Collections.Generic;
using BetterTTD.Domain.Entities.Base;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Domain.Entities
{
    public class Company : Poolable<int>
    {
        public const int INVALID_COMPANY   = 255;
        public const int COMPANY_SPECTATOR = 255;
        public const int MAX_COMPANIES     = 15;

        public string Name { get; set; }
        public string President { get; set; }
        public long Inaugurated { get; set; }
        public long Value { get; set; }
        public long Income { get; set; }
        public int Performance { get; set; }
        public bool IsPassworded { get; set; }
        public bool IsAI { get; set; }
        public Colors Color { get; set; }
        public int[] Shares { get; set; } = {INVALID_COMPANY, INVALID_COMPANY, INVALID_COMPANY, INVALID_COMPANY};
        public int Bankruptcy { get; set; }

        public Economy CurrentEconomy { get; set; } = new Economy();
        public LinkedList<Economy> EconomyHistory { get; set; } = new LinkedList<Economy>();
        public readonly Dictionary<VehicleType, int> Vehicles = new Dictionary<VehicleType, int>();
        public readonly Dictionary<VehicleType, int> Stations = new Dictionary<VehicleType, int>();
        
        public Company(int id) : base(id)
        {
            if (IsSpectator())
            {
                Name = "Spectator";
            }
        }

        public bool IsSpectator()
        {
            return IsSpectator(Id);
        }
        
        public static bool IsSpectator(int id)
        {
            return id == COMPANY_SPECTATOR;
        }

        public static bool IsValid(int id)
        {
            return id < MAX_COMPANIES;
        }

        public static bool IsValidOrSpectator(int id)
        {
            return IsValid(id) || IsSpectator(id);
        }
    }
}