using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Backend
{
    public class PlayerAttackProvidor : DamageEntityProvidor<PlayerAttackStats>
    {
        private Dictionary<string, PlayerAttackStats> _playerAttackStats => _damageEntities;

        public PlayerAttackProvidor(): base(nameof(PlayerAttackStats))
        {
        }

        public virtual PlayerAttackStats GetPlayerAttackStats(string weaponName) => GetDamageEntityStats(weaponName);
    }
}
