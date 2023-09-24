using System;

namespace Assets.Script.Backend
{
    public class InventoryItem
    {
        private ThreadSafeIntStats _count { get; set; }

        public long Id { get; set; }
        public string Type { get; set; }
        public int CurrentCount => _count.CurrentStats;
        public int MaxCount => _count.MaxStats;

        public InventoryItem(int maxCount, int currentCount = 0)
        {
            _count = new ThreadSafeIntStats("Count", minStats: 0, maxCount, currentCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns>total count added to inventory</returns>
        public virtual int AddItems(int count)
        {
            var addedCount = Math.Min(MaxCount - CurrentCount, count);
            _count.UpdateStats(addedCount);
            EventManager.TriggerEvent(GameEventTypes.GetItemReachedMaxCountEvent);
            return addedCount;
        }

        public virtual void RemoveItems(int count)
        {
            if (CurrentCount - count < 0)
            {
                throw new Exception($"Not enough items in inventory. Requested: {count}, available: {CurrentCount}");
            }
            _count.UpdateStats(-count);
        }

        public virtual void UseItem()
        {
            if (CurrentCount - 1 < 0)
            {
                throw new Exception($"Not enough items in inventory. Requested: {1}, available: {CurrentCount}");
            }
            GameEventLogger.LogEvent($"Using item Id: {Id}");
            ItemEffect();
            _count.UpdateStats(-1);
        }

        public virtual void ItemEffect() => throw new NotImplementedException();

        public InventoryItem Clone()
        {
            return new InventoryItem(MaxCount, CurrentCount)
            {
                Id = Id,
                Type = Type,
            };
        }
    }
}
