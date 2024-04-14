using UnityEngine;

namespace Assets.Script.Backend
{
    [CreateAssetMenu(fileName = "New Weapon Stats", menuName = "WeaponStats/CreateWeapon")]
    public class WeaponStats : ScriptableObject
    {
        public string WeaponName;

        public int WeaponAttack;

        public float SimpleDamageMultiplier;

        public virtual float GetDamageMultiplier()
        {
            return SimpleDamageMultiplier;
        }
    }
}
