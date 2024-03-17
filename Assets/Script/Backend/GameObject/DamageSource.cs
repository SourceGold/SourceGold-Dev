namespace Assets.Script.Backend
{
    public class DamageSource
    {
        public HittableObjectType SrcObjectType { get; set; }

        public string SrcObjectName { get; set; }

        public string AttackWeapon { get; set; } = string.Empty;

        public string AttackName { get; set; }
    }

    public class DamageTarget
    {
        public HittableObjectType TgtObjectType { get; set; }

        public string TgtObjectName { get; set; }

        public string HitArmor { get; set; }
    }
}
