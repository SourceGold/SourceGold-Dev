using System;
using System.Threading;

namespace Assets.Script.Backend
{
    public class HittableObject : GameObject
    {

        public HittableObjectType HittableObjectType { get; set; }

        public HittableObjectStats HittableObjectStats { get; set; }

        public GameObjectEnvironmentalStats EnvironmentalStats { get; set; }

        public HittableObject(string name, HittableObjectStats hittableObjectStats, 
            HittableObjectType hittableObjectType, 
            GameObjectEnvironmentalStats environmentalStats)
            : base(name, GameObjectType.HittableObject)
        {
            HittableObjectStats = hittableObjectStats;
            HittableObjectType = hittableObjectType;
            EnvironmentalStats = environmentalStats;
        }

        public virtual void GotHit(int incomingDmg, EventLogger logger)
        {
            HittableObjectStats.GotHit(incomingDmg, logger);
            
            if (!this.HittableObjectStats.IsAlive)
            {
                EventManager.TriggerEvent($"{Name}Death");
                logger.LogEvent($"{Name} is died");
            }
        }

        public bool IsAlive => HittableObjectStats.IsAlive;
    }

    public class HittableObjectStats
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
            logger.LogEvent($"{dmg} damage dealt");
        }

        public bool IsAlive => CurrentHp > 0;
    }
}
