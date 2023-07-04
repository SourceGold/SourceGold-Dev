using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Script.Backend
{
    public class HittableObject : GameObject
    {

        public HittableObjectType HittableObjectType { get; set; }

        public HittableObjectStats HittableObjectStats { get; set; }

        public HittableObject(string name, HittableObjectStats hittableObjectStats, HittableObjectType hittableObjectType) : base(name, GameObjectType.HittableObject)
        {
            HittableObjectStats = hittableObjectStats;
            HittableObjectType = hittableObjectType;
        }

        public virtual void GotHit(int incomingDmg)
        {
            HittableObjectStats.GotHit(incomingDmg);
            if (!this.HittableObjectStats.IsAlive)
            {
                EventManager.TriggerEvent($"{Name}Death");
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

        public virtual void GotHit(int incomingDmg)
        {
            int dmg = Math.Max(incomingDmg - Defense, 1);
            
            Interlocked.Add(ref CurrentHp, -dmg);
        }

        public bool IsAlive => CurrentHp > 0;
    }
}
