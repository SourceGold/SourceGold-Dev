using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Backend
{
    public class DamangeSource
    {
        public HittableObjectType SrcObjectType { get; set; }

        public string SrcObjectName { get; set; }

        public string AttackWeapon { get; set; }

        public string AttackName { get; set; }
    }

    public class DamageTarget
    {
        public HittableObjectType TgtObjectType { get; set; }

        public string TgtObjectName { get; set; }

        public string HitArmor { get; set; }
    }
}
