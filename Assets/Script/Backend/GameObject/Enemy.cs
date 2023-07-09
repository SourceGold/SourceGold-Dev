namespace Assets.Script.Backend
{
    public class Enemy : HittableObject
    {
        public Enemy(string name, EnemyStats enemyStats, GameObjectEnvironmentalStats environmentalStats)
            : base(name, enemyStats, HittableObjectType.Enemy, environmentalStats)
        {
        }

        public override void GotHit(int incomingDmg, EventLogger logger)
        {
            base.GotHit(incomingDmg, logger);
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
