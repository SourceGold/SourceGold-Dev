using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Backend
{
    public class DamageEntityProvidor<T> where T : DamageEntity
    {
        protected Dictionary<string, T> _damageEntities { get; set; }

        protected DamageEntityProvidor(string path)
        {
            _damageEntities = Resources.LoadAll<T>(path).ToDictionary(x => x.name);
        }

        protected T GetDamageEntityStats(string entityName)
        {
            return _damageEntities[entityName];
        }
    }
}
