using System;

namespace Assets.Script.Backend
{
    public class Enemy : HittableObject
    {
        public EnemyStats EnemyStats => HittableObjectStats as EnemyStats;

        public Enemy(string name, EnemyStats enemyStats, bool saveToNextStage = false)
            : base(name, enemyStats, HittableObjectType.Enemy, saveToNextStage)
        {
        }

        public Enemy(string name, bool saveToNextStage = false)
            : base(name, HittableObjectType.Enemy, saveToNextStage)
        {
        }

        public override void GotDamanged(int incomingDmg)
        {
            base.GotDamanged(incomingDmg);
        }

        public void SetOnStatsChangedCallback(Action<EnemyStats> onStatsChangedCallback, bool enableOnStatsChangedCallback)
        {
            EnableOnStatsChangedCallback = enableOnStatsChangedCallback;
            EnemyStats.OnStatsChangedCallback = onStatsChangedCallback;
        }
    }

    public class EnemyStats : HittableObjectStats
    {
        public int Level { get; set; }

        private Action<EnemyStats> _onStatsChangedCallback;

        public Action<EnemyStats> OnStatsChangedCallback
        {
            protected get
            {
                return _onStatsChangedCallback;
            }
            set
            {
                _onStatsChangedCallback = value;
                if (EnableOnStatsChangedCallback)
                {
                    _onStatsChangedCallback(this);
                }
            }
        }

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
