using System;

namespace Assets.Script.Backend
{
    public class ThreadSafeIntStats : ThreadSafeStats<int>
    {
        public ThreadSafeIntStats(string statsName, int minStats, int maxStats, int currentStats) : base(statsName, minStats, maxStats, currentStats)
        {
        }

        public override int CalculateCurrentStats(int changeInStats)
        {
            return Math.Min(Math.Max(MinStats, CurrentStats + changeInStats), MaxStats);
        }
    }
}
