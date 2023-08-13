using System;

namespace Assets.Script.Backend
{
    public class ThreadSafeIntStats: ThreadSafeStats<int>
    {
        public ThreadSafeIntStats(string statsName, int minStats, int maxStats, int currentStats) : base(statsName, minStats, maxStats, currentStats)
        {
        }

        public override void UpdateStats(int changeInStats)
        {
            lock (Lock)
            {
                CurrentStats = Math.Min(Math.Max(MinStats, CurrentStats + changeInStats), MaxStats);
                GameEventLogger.LogEvent($"Updating Stats: {StatsName} by: {changeInStats}");
            }
        }
    }
}
