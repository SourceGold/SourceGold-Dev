using System;
using System.Threading;

#nullable enable

namespace Assets.Script.Backend
{
    public class HittableObject : GameObject
    {
        public HittableObjectType HittableObjectType { get; set; }

        public HittableObjectStats? HittableObjectStats { get; set; }

        public HittableObject(string name,
            HittableObjectStats hittableObjectStats,
            HittableObjectType hittableObjectType,
            GameObjectEnvironmentalStats environmentalStats,
            bool saveToNextStage)
            : base(name, GameObjectType.HittableObject, environmentalStats, saveToNextStage)
        {
            HittableObjectStats = hittableObjectStats;
            HittableObjectType = hittableObjectType;
        }

        public HittableObject(string name,
            HittableObjectType hittableObjectType,
            GameObjectEnvironmentalStats environmentalStats,
            bool saveToNextStage)
            : base(name, GameObjectType.HittableObject, environmentalStats, saveToNextStage)
        {
            HittableObjectType = hittableObjectType;
        }

        public override void SetGameObjectStates(GameObjectStats gameObjectStats)
        {
            if (gameObjectStats is HittableObjectStats hittableObjectStats)
            {
                HittableObjectStats = hittableObjectStats;
            }
        }

        public override GameObjectStats GetGameObjectStates()
        {
            return HittableObjectStats!;
        }

        public virtual void GotHit(int incomingDmg, EventLogger logger)
        {
            HittableObjectStats!.GotHit(incomingDmg);

            if (!this.HittableObjectStats.IsAlive)
            {
                EventManager.TriggerEvent(GameEventTypes.GetObjectOnDeathEvent(this.Name));
            }
        }

        public bool IsAlive => HittableObjectStats!.IsAlive;
    }

    public class HittableObjectStats : GameObjectStats
    {
        public int MaxHitPoint { get; private set; }

        public int AttackDmg { get; private set; }

        public int Defense { get; private set; }

        public int CurrentHp = -1;

        public HittableObjectStats(int maxHitPoint = 100, int attackDmg = 0, int defense = 0)
        {
            MaxHitPoint = maxHitPoint;
            AttackDmg = attackDmg;
            Defense = defense;
            CurrentHp = maxHitPoint;
        }

        public virtual void GotHit(int incomingDmg, EventLogger logger)
        {
            int dmg = Math.Max(incomingDmg - Defense, 1);

            Interlocked.Add(ref CurrentHp, -dmg);
            GameEventLogger.LogEvent($"{dmg} damage dealt");
        }

        public bool IsAlive => CurrentHp > 0;
    }
}
