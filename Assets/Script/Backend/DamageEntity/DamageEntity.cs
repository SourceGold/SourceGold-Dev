using UnityEngine;

namespace Assets.Script.Backend
{
    public class DamageEntity : ScriptableObject
    {
        public float SimpleDamageMultiplier;

        public virtual float GetDamageMultiplier()
        {
            return SimpleDamageMultiplier;
        }
    }
}
