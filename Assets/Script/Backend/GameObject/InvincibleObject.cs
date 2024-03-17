﻿namespace Assets.Script.Backend
{
    public class InvincibleObject : BackendGameObject
    {
        public InvincibleObjectType InvincibleObjectType { get; set; }

        public InvincibleObject(string name,
            InvincibleObjectType invincibleObjectType,
            GameObjectEnvironmentalStats environmentalStats,
            bool saveToNextStage)
            : base(name, GameObjectType.InvincibleObject, environmentalStats, saveToNextStage)
        {
            InvincibleObjectType = invincibleObjectType;
        }

        public override void SetGameObjectStats(BackendGameObjectStats GameObjectStats)
        {
            return;
        }

        public override BackendGameObjectStats GetGameObjectBaseStats()
        {
            return null;
        }
    }
}
