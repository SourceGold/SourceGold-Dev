namespace Assets.Script.Backend
{
    public class InvincibleObject : GameObject
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

        public override void SetGameObjectStates(GameObjectStats GameObjectStats)
        {
            return;
        }

        public override GameObjectStats GetGameObjectStates()
        {
            return null;
        }
    }
}
