using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Backend
{
    public class WeaponProvidor
    {
        private Dictionary<string, WeaponStats> _weaponStats { get; set; }

        public WeaponProvidor()
        {
            _weaponStats = Resources.LoadAll<WeaponStats>("WeaponStats").ToDictionary(x => x.WeaponName);
        }

        public virtual WeaponStats GetWeaponStats(string weaponName)
        {
            return _weaponStats[weaponName];
        }
    }
}
