using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Backend
{
    public class WeaponProvidor : DamageEntityProvidor<WeaponStats>
    {
        private Dictionary<string, WeaponStats> _weaponStats => _damageEntities;

        public WeaponProvidor(): base(nameof(WeaponStats))
        {
        }

        public virtual WeaponStats GetWeaponStats(string weaponName) => GetDamageEntityStats(weaponName);
    }
}
