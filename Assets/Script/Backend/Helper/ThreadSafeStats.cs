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
        private bool _enableOnStatsChangedCallback { get; set; } = false;

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
                if (_enableOnStatsChangedCallback)
                {
                    OnStatsChangedCallback();
                }
            }
            GameEventLogger.LogEvent($"Updating Stats: {StatsName} by: {changeInStats}");
        }

        public void SetOnStatsChangedCallback(Action onStatsChangedCallback)
        {
            OnStatsChangedCallback = onStatsChangedCallback;
        }

        public void SetEnableOnStatsChangedCallback(bool enableOnStatsChangedCallback)
        {
            _enableOnStatsChangedCallback = enableOnStatsChangedCallback;
        }

        public abstract T CalculateCurrentStats(T changeInStats);

        protected Action OnStatsChangedCallback { get; set; }
    }
}
