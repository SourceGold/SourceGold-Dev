using System;

namespace Assets.Script.Backend
{
    public class ThreadSafeDoubleStats : ThreadSafeStats<double>
    {
        public ThreadSafeDoubleStats(string statsName, double minStats, double maxStats, double currentStats) : base(statsName, minStats, maxStats, currentStats)
        {
        }

        public override double CalculateCurrentStats(double changeInStats)
        {
            return Math.Min(Math.Max(MinStats, CurrentStats + changeInStats), MaxStats);
        }
    }
}
