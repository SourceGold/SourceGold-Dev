using System;
using System.Threading;

namespace Assets.Script.Backend
{
    public class PlayableCharacter : HittableObject
    {
        private bool _isMainCharacter { get; set; }

        public PlayableCharacterStats PlayableCharacterStats => HittableObjectStats as PlayableCharacterStats;

        public bool IsMainCharacter
        {
            get
            {
                return _isMainCharacter;
            }
            set
            {
                _isMainCharacter = value;
                EnableOnStatsChangedCallback = value;
            }
        }

        public PlayableCharacter(string name, PlayableCharacterStats characterStats, GameObjectEnvironmentalStats environmentalStats, bool isMainCharacter)
            : base(name, characterStats, HittableObjectType.PlayableCharacter, environmentalStats, saveToNextStage: true)
        {
            IsMainCharacter = isMainCharacter;
        }

        public PlayableCharacter(string name, GameObjectEnvironmentalStats environmentalStats)
            : base(name, HittableObjectType.PlayableCharacter, environmentalStats, saveToNextStage: true)
        {
        }

        public PlayableCharacter(string name)
            : base(name, HittableObjectType.PlayableCharacter, null, saveToNextStage: true)
        {
        }

        public override void GotDamanged(int incomingDmg)
        {
            base.GotDamanged(incomingDmg);
        }

        public void SetOnStatsChangedCallback(Action<PlayableCharacterStats> onStatsChangedCallback)
        {
            PlayableCharacterStats.OnStatsChangedCallback = onStatsChangedCallback;
            EnableOnStatsChangedCallback = _isMainCharacter;
        }
    }

    public class PlayableCharacterStats : HittableObjectStats
    {
        public int Level { get; set; }

        public int CurrentExp = 0;

        public const int LevelUpExp = 100;

        public Action<PlayableCharacterStats> OnStatsChangedCallback { get; set; }

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

        protected override void OnStatsChanged()
        {
            OnStatsChangedCallback(this);
        }

        public virtual void GotExp(int Exp)
        {
            Interlocked.Add(ref CurrentExp, Exp);
            Level = CurrentExp / LevelUpExp + 1;
        }
    }
}
