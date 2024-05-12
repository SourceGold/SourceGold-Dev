using Assets.Script.Backend;
using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameItemDynamic
{
    private ThreadSafeIntStats _count { get; set; }
    public string uid { get; set; }
    public string id { get; set; }
    public int CurrentCount => _count.CurrentStats;
    public int MaxCount => _count.MaxStats;

    public int level;
    public bool isNew;
    public bool dummy;
    public GameItem staticInfo;
    public Dictionary<string, int> additionalIntStats = new Dictionary<string, int>();
    public Dictionary<string, int> additionalIntStatsHidden = new Dictionary<string, int>();
    public Dictionary<string, float> additionalFloatStats = new Dictionary<string, float>();
    public Dictionary<string, float> additionalFloatStatsHidden = new Dictionary<string, float>();
    public List<string> addtionalStatus = new List<string>();
    public List<string> addtionalStatusHidden = new List<string>();

    public override bool Equals(object obj) => Equals(obj as GameItemDynamic);

    public bool Equals(GameItemDynamic p)
    {
        if (p is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (ReferenceEquals(this, p))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (GetType() != p.GetType())
        {
            return false;
        }
        // Return true if the fields match.
        // Note that the base class is not invoked because it is
        // System.Object, which defines Equals as reference equality.
        if (id.ToLower() != p.id.ToLower())
            return false;
        if (p.dummy)
            return true;
        if (level != p.level) 
            return false;
        if (additionalFloatStats.Count != p.additionalFloatStats.Count)
            return false;
        if (additionalIntStats.Count != p.additionalIntStats.Count)
            return false;
        if (additionalIntStats.Count != p.additionalIntStats.Count)
            return false;
        foreach (var floatStat in additionalFloatStats)
        {
            if (!p.additionalFloatStats.ContainsKey(floatStat.Key))
                return false;
            if (floatStat.Value != p.additionalFloatStats[floatStat.Key]) 
                return false;
        }

        foreach (var intStat in additionalIntStats)
        {
            if (!p.additionalIntStats.ContainsKey(intStat.Key))
                return false;
            if (intStat.Value != p.additionalIntStats[intStat.Key])
                return false;
        }
        foreach (var stat in addtionalStatus)
        {
            if (!p.addtionalStatus.Contains(stat))
                return false;
        }
        return true;
    }
    public GameItemDynamic(string id, int currentCount = 1, int level = 0, bool isNew = true, bool dummy = false)
    {
        this.id = id;
        this.uid = System.Guid.NewGuid().ToString();
        this.dummy = dummy;
        if (dummy) return;
        staticInfo = GameItemsStaticManager.Instance.GetGameItem(id);

        if (staticInfo.maximum_count <= 0)
            _count = new ThreadSafeIntStats("Count", minStats: 0, Int32.MaxValue, currentCount);
        else
            _count = new ThreadSafeIntStats("Count", minStats: 0, staticInfo.maximum_count, currentCount);

        this.level = level;
        this.isNew = isNew;
        ConsumableController.Instance.activateItem(this);
    }

    public GameItemDynamic(string id, Dictionary<string, string> additionalConfig)
    {
        this.id = id;
        this.uid = System.Guid.NewGuid().ToString();

        if (additionalConfig.ContainsKey("dummy") && additionalConfig["dummy"] == "true")
        {
            this.dummy = true;
            return;
        } else
        {
            this.dummy = false;
        }

        if (!additionalConfig.ContainsKey("current_count"))
            throw new Exception("count for none dummy object is 0");

        staticInfo = GameItemsStaticManager.Instance.GetGameItem(id);
        if (staticInfo.maximum_count <= 0)
            _count = new ThreadSafeIntStats("Count", minStats: 0, Int32.MaxValue, Int32.Parse(additionalConfig["current_count"]));
        else
            _count = new ThreadSafeIntStats("Count", minStats: 0, staticInfo.maximum_count, Int32.Parse(additionalConfig["current_count"]));


        if (additionalConfig.ContainsKey("is_new") && additionalConfig["is_new"] == "false")
            this.isNew = false;
        else
            this.isNew = true;

        if (additionalConfig.ContainsKey("level"))
            this.level = Int32.Parse(additionalConfig["level"]);

        ConsumableController.Instance.activateItem(this, additionalConfig);
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
