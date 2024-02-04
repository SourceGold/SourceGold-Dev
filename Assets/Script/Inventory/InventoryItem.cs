using Assets.Script.Backend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    private ThreadSafeIntStats _count { get; set; }
    public string uid { get; set; }
    public string id { get; set; }
    public int CurrentCount => _count.CurrentStats;
    public int MaxCount => _count.MaxStats;

    public int level;
    public bool isNew;

    public GameItem staticInfo;
    public InventoryItem(string id, int maxCount = 10000, int currentCount = 1, int level = 0, bool isNew = true)
    {
        staticInfo = GameItemsStaticManager.Instance.GetGameItem(id);
        if (staticInfo.maximum_count == 0)
            _count = new ThreadSafeIntStats("Count", minStats: 0, int.MaxValue, currentCount);
        else
            _count = new ThreadSafeIntStats("Count", minStats: 0, staticInfo.maximum_count, currentCount);

        this.id = id;
        this.uid = System.Guid.NewGuid().ToString();
        this.level = level;
        this.isNew = isNew;
    }
    public virtual int AddItems(int count)
    {
        var addedCount = Math.Min(MaxCount - CurrentCount, count);
        _count.UpdateStats(addedCount);
        return count - addedCount;
    }

    public virtual void RemoveItems(int count)
    {
        if (CurrentCount - count < 0)
        {
            throw new Exception($"Not enough items in inventory. Requested: {count}, available: {CurrentCount}");
        }
        _count.UpdateStats(-count);
    }

    public override string ToString()
    {
        return $"ID is {id}, level is {level}, GUID is {uid}, count is {CurrentCount}";
    }


}
