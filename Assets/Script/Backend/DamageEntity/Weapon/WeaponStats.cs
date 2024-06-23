using UnityEngine;

namespace Assets.Script.Backend
{
    [CreateAssetMenu(fileName = "New Weapon Stats", menuName = "WeaponStats/Create Weapon")]
    public class WeaponStats : DamageEntity
    {
        public string WeaponName => this.name;

        public int WeaponAttack;
    }
}
