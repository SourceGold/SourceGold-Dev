﻿using System.Threading;

namespace Assets.Script.Backend
{
    public class PlayableCharacter : HittableObject
    {
        public PlayableCharacter(string name, PlayableCharacterStats characterStats, GameObjectEnvironmentalStats environmentalStats)
            : base(name, characterStats, HittableObjectType.PlayableCharacter, environmentalStats)
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

        public PlayableCharacterStats(int maxHitPoint, int attackDmg, int defense, int level = 1, int currentExp = 0) : base(maxHitPoint, attackDmg, defense)
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
