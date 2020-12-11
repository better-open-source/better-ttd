using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Coan_OLD.Networks
{
    public class Protocol
    {
        public int Version { get; set; } = -1;
        private readonly Dictionary<AdminUpdateType, ArrayList> _supportedFrequencies;

        public Protocol()
        {
            _supportedFrequencies = new Dictionary<AdminUpdateType, ArrayList>();
        }

        public void AddSupport(int typeIndex, int freqIndex)
        {
            var freq = (AdminUpdateFrequency)freqIndex;
            var type = (AdminUpdateType)typeIndex;

            if (_supportedFrequencies.Keys.Contains(type) == false)
            {
                _supportedFrequencies.Add(type, new ArrayList());
            }
            
            _supportedFrequencies[type].Add(freq);
        }

        public bool IsSupported(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            return _supportedFrequencies[type].Contains(freq);
        }
    }
}