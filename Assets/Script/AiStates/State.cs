using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : ScriptableObject
{
    public virtual State Tick(CharacterManager characterManager)
    {
        return this;
    }
}
