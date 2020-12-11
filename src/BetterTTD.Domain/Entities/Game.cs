using System.Collections.Generic;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Domain.Entities
{
    public class Game
    {
        public string Name { get; set; }
        public string GameVersion { get; set; }
        public int VersionProtocol { get; set; }
        public bool Dedicated { get; set; }
        public Map Map { get; set; } = new();
        public Dictionary<PauseMode, bool> PauseState { get; } = new();

        public bool IsPaused ()
        {
            return PauseState.ContainsValue(true);
        }

        public void SetPauseMode(PauseMode pm, bool paused)
        {
            if (paused == false)
            {
                PauseState.Remove(pm);
            }
            else
            {
                PauseState.Add(pm, true);
            }
        }

        public PauseMode GetPauseMode ()
        {
            foreach (var (key, value) in PauseState)
            {
                if (value)
                {
                    return key;
                }
            }
        
            return PauseMode.PM_UNPAUSED;
        }
    }
}