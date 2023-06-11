using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Backend
{
    // not sure if those enums are needed
    public enum GameObjectType
    {
        _,
        HittableObject,
        NonHittableObject,
        Items
    }

    public enum HittableObjectType
    {
        _,
        PlayableCharacter,
        Enemy,
        HittableNpc,
        StationaryObject
    }
}
