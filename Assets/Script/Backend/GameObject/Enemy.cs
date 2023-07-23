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

        public EnemyStats(int maxHitPoint, int attackDmg, int defense, int level = 1) : base(maxHitPoint, attackDmg, defense)
        {
            Level = level;
        }
    }
}
