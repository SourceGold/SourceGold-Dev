using System;

namespace Assets.Script.Backend
{
    public class Enemy : HittableObject
    {
        public EnemyStats EnemyStats => HittableObjectStats as EnemyStats;

        public Enemy(string name, EnemyStats enemyStats, GameObjectEnvironmentalStats environmentalStats, bool saveToNextStage = false)
            : base(name, enemyStats, HittableObjectType.Enemy, environmentalStats, saveToNextStage)
        {
        }

        public Enemy(string name, GameObjectEnvironmentalStats environmentalStats, bool saveToNextStage = false)
            : base(name, HittableObjectType.Enemy, environmentalStats, saveToNextStage)
        {
        }

        public Enemy(string name, bool saveToNextStage = false)
            : base(name, HittableObjectType.Enemy, null, saveToNextStage)
        {
        }

        public override void GotDamanged(int incomingDmg)
        {
            base.GotDamanged(incomingDmg);
        }

        public void SetOnStatsChangedCallback(Action<EnemyStats> onStatsChangedCallback, bool enableOnStatsChangedCallback)
        {
            EnemyStats.OnStatsChangedCallback = onStatsChangedCallback;
            EnableOnStatsChangedCallback = enableOnStatsChangedCallback;
        }
    }

    public class EnemyStats : HittableObjectStats
    {
        public int Level { get; set; }

        public Action<EnemyStats> OnStatsChangedCallback { get; set; }

        public EnemyStats(string parentName, int maxHitPoint, int maxMagicPoint, int baseAttack, int baseDefence, int level = 1) :
            base(parentName, maxHitPoint, maxMagicPoint, maxStamina: 0, baseAttack, baseDefence)
        {
            Level = level;
        }

        protected override void OnStatsChanged()
        {
            OnStatsChangedCallback(this);
        }
    }
}
