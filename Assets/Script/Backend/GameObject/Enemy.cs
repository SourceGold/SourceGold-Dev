namespace Assets.Script.Backend
{
    public class Enemy : HittableObject
    {
        public Enemy(string name, EnemyStats enemyStats, GameObjectEnvironmentalStats environmentalStats, bool saveToNextStage = false)
            : base(name, enemyStats, HittableObjectType.Enemy, environmentalStats, saveToNextStage)
        {
        }

        public Enemy(string name, GameObjectEnvironmentalStats environmentalStats, bool saveToNextStage = false)
            : base(name, HittableObjectType.Enemy, environmentalStats, saveToNextStage)
        {
        }

        public override void GotHit(int incomingDmg)
        {
            base.GotHit(incomingDmg);
        }
    }

    public class EnemyStats : HittableObjectStats
    {
        public int Level { get; set; }

        public EnemyStats(string parentName, int maxHitPoint, int maxMagicPoint, int baseAttack, int baseDefence, int level = 1) :
            base(parentName, maxHitPoint, maxMagicPoint, maxStamina: 0, baseAttack, baseDefence)
        {
            Level = level;
        }
    }
}
