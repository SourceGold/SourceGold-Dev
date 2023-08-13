using System.Threading;

namespace Assets.Script.Backend
{
    public class PlayableCharacter : HittableObject
    {
        public PlayableCharacter(string name, PlayableCharacterStats characterStats, GameObjectEnvironmentalStats environmentalStats)
            : base(name, characterStats, HittableObjectType.PlayableCharacter, environmentalStats, saveToNextStage: true)
        {
        }

        public PlayableCharacter(string name, GameObjectEnvironmentalStats environmentalStats)
            : base(name, HittableObjectType.PlayableCharacter, environmentalStats, saveToNextStage: true)
        {
        }

        public override void GotHit(int incomingDmg)
        {
            base.GotHit(incomingDmg);
        }
    }

    public class PlayableCharacterStats : HittableObjectStats
    {
        public int Level { get; set; }

        public int CurrentExp = 0;

        public const int LevelUpExp = 100;

        public PlayableCharacterStats(
            string parentName, 
            int maxHitPoint, 
            int maxMagicPoint,
            int maxStamina,
            int baseAttack, 
            int baseDefense, 
            int level = 1, 
            int currentExp = 0) 
            : base(parentName, maxHitPoint, maxMagicPoint, maxStamina, baseAttack, baseDefense)
        {
            Level = level;
            CurrentExp = currentExp;
        }

        public virtual void GotExp(int Exp)
        {
            Interlocked.Add(ref CurrentExp, Exp);
            Level = CurrentExp / LevelUpExp + 1;
        }
    }
}
