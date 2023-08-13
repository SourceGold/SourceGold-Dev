using System;

namespace Assets.Script.Backend
{
    public abstract class ThreadSafeStats<T>
    {
        protected readonly object Lock = new object();
        public string StatsName { get; protected set; }
        public T MinStats { get; protected set; }
        public T MaxStats { get; protected set; }
        public T CurrentStats { get; protected set; }

        public ThreadSafeStats(string statsName, T minStats, T maxStats, T currentStats)
        {
            StatsName = statsName;
            MinStats = minStats;
            MaxStats = maxStats;
            CurrentStats = currentStats;
        }

        public virtual void UpdateStats(T changeInStats)
        {
            lock (Lock)
            {
                CurrentStats = CalculateCurrentStats(changeInStats);
            }
            GameEventLogger.LogEvent($"Updating Stats: {StatsName} by: {changeInStats}");
        }

        public abstract T CalculateCurrentStats(T changeInStats);
    }
}
