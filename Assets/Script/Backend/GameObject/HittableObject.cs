using System;

#nullable enable

namespace Assets.Script.Backend
{
    public abstract class HittableObject : BackendGameObject
    {
        public HittableObjectType HittableObjectType { get; set; }

        public HittableObjectStats? HittableObjectStats { get; set; }

        public HittableObject(string name,
            HittableObjectStats hittableObjectStats,
            HittableObjectType hittableObjectType,
            bool saveToNextStage)
            : base(name, GameObjectType.HittableObject, saveToNextStage)
        {
            HittableObjectStats = hittableObjectStats;
            HittableObjectType = hittableObjectType;
        }

        public HittableObject(string name,
            HittableObjectType hittableObjectType,
            bool saveToNextStage)
            : base(name, GameObjectType.HittableObject, saveToNextStage)
        {
            HittableObjectType = hittableObjectType;
        }

        public override void SetGameObjectStates(BackendGameObjectStats gameObjectStats)
        {
            if (gameObjectStats is HittableObjectStats hittableObjectStats)
            {
                HittableObjectStats = hittableObjectStats;
            }
        }

        public override BackendGameObjectStats GetGameObjectStates()
        {
            return HittableObjectStats!;
        }

        public virtual void GotDamanged(int incomingDmg)
        {
            HittableObjectStats!.GotDamanged(incomingDmg);
        }

        public bool IsAlive => HittableObjectStats!.IsAlive;

        public bool EnableOnStatsChangedCallback
        {
            set => HittableObjectStats!.EnableOnStatsChangedCallback = value;
        }
    }

    public abstract class HittableObjectStats : BackendGameObjectStats
    {
        private bool _enableOnStatsChangedCallback = false;

        protected readonly string HitPointName = "HitPoint";

        protected readonly string MagicPointName = "MagicPoint";

        protected readonly string StaminaName = "Stamina";

        protected ThreadSafeDoubleStats _hp { get; set; }

        protected ThreadSafeDoubleStats _mp { get; set; }

        protected ThreadSafeDoubleStats _stamina { get; set; }

        public double BaseAttack { get; private set; }

        public double BaseDefense { get; private set; }



        public int MaxHitPoint => (int)Math.Round(_hp.MaxStats);

        public int MaxMagicPoint => (int)Math.Round(_mp.MaxStats);

        public int MaxStamina => (int)Math.Round(_stamina.MaxStats);

        public int CurrentHitPoint => (int)Math.Round(_hp.CurrentStats);

        public int CurrentMagicPoint => (int)Math.Round(_mp.CurrentStats);

        public int CurrentStamina => (int)Math.Round(_stamina.CurrentStats);

        public int Attack => (int)CalculateAttack();

        public int Defense => (int)CalculateDefense();

        public bool EnableOnStatsChangedCallback 
        {
            protected get
            {
                return _enableOnStatsChangedCallback;
            }
            set
            {
                _enableOnStatsChangedCallback = value;
                _hp.SetEnableOnStatsChangedCallback(value);
                _mp.SetEnableOnStatsChangedCallback(value);
                _stamina.SetEnableOnStatsChangedCallback(value);
            }
        }

        public HittableObjectStats(
            string parentName,
            int maxHitPoint = 100,
            int maxMagicPoint = 100,
            int maxStamina = 100,
            int baseAttack = 10,
            int baseDefense = 0
            ) : base(parentName)
        {
            _hp = new ThreadSafeDoubleStats(statsName: HitPointName, minStats: 0, maxStats: maxHitPoint, currentStats: maxHitPoint);
            _mp = new ThreadSafeDoubleStats(statsName: MagicPointName, minStats: 0, maxStats: maxMagicPoint, currentStats: maxMagicPoint);
            _stamina = new ThreadSafeDoubleStats(statsName: StaminaName, minStats: 0, maxStats: maxStamina, currentStats: maxStamina);
            _hp.SetOnStatsChangedCallback(OnStatsChanged);
            _mp.SetOnStatsChangedCallback(OnStatsChanged);
            _stamina.SetOnStatsChangedCallback(OnStatsChanged);
            BaseAttack = baseAttack;
            BaseDefense = baseDefense;
        }

        public virtual void GotDamanged(int incomingDmg)
        {
            int dmg = (int)Math.Round(CalculateDamage(incomingDmg));
            GameEventLogger.LogEvent($"{nameof(GotDamanged)}: {dmg} damage dealt");
            UpdateHitPoint(-dmg);
        }

        public virtual void UpdateHitPoint(int changeInHp)
        {
            GameEventLogger.LogEvent($"{nameof(UpdateHitPoint)}: {changeInHp} {HitPointName} used");
            _hp.UpdateStats(changeInHp);
            if (!IsAlive)
            {
                EventManager.TriggerEvent(GameEventTypes.GetObjectOnDeathEvent(BackendGameObjectName));
            }
        }

        public virtual void UpdateMagicPoint(int changeInMp)
        {
            if (CurrentMagicPoint + changeInMp < 0)
            {
                EventManager.TriggerEvent(GameEventTypes.GetNotEnoughStatsEvent(nameof(CurrentMagicPoint)));
            }
            else
            {
                GameEventLogger.LogEvent($"{nameof(UpdateMagicPoint)}: {changeInMp} {MagicPointName} used");
                _mp.UpdateStats(changeInMp);
            }
        }

        public virtual void UpdateStamina(int changeInStamina)
        {
            if (CurrentStamina + changeInStamina < 0)
            {
                EventManager.TriggerEvent(GameEventTypes.GetNotEnoughStatsEvent(nameof(CurrentStamina)));
            }
            else
            {
                GameEventLogger.LogEvent($"{nameof(UpdateMagicPoint)}: {changeInStamina} {StaminaName} used");
                _stamina.UpdateStats(changeInStamina);
            }
        }

        public bool IsAlive => CurrentHitPoint > 0;

        protected abstract void OnStatsChanged();

        protected virtual double CalculateAttack()
        {
            return Math.Round(BaseAttack);
        }

        protected virtual double CalculateDefense()
        {
            return Math.Round(BaseDefense);
        }

        protected virtual double CalculateDamage(int incomingDmg)
        {
            return Math.Max(incomingDmg - CalculateDefense(), 1);
        }
    }
}
