using UnityEngine;

namespace Assets.Script.Backend
{
    [CreateAssetMenu(fileName = "New Player Attack Stats", menuName = "PlayerAttackStats/Create PlayerStats")]
    public class PlayerAttackStats : DamageEntity
    {
        public string PlayerAttackName => this.name;
    }
}
