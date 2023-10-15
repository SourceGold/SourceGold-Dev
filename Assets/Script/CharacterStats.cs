using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private int _hitPointLevel = 10;
    public int HitPointLevel
    {
        get { return _hitPointLevel; }
        set { _hitPointLevel = value; } 
    }

    private int _maxHitPoint;
    public int MaxHitPoint
    {
        get { return _maxHitPoint; }
        set { _maxHitPoint = value; }
    }

    private int _currentHitPoint;
    public int CurrentHitPoint
    {
        get { return _currentHitPoint; }
        set { _currentHitPoint = value; }
    }

    private int _staminaLevel = 10;
    public int StaminaLevel
    {
        get { return _staminaLevel; }
        set { _staminaLevel = value; }
    }

    private int _maxStamina;
    public int MaxStamina
    {
        get { return _maxStamina; }
        set { _maxStamina = value; }
    }

    private int _currentStamina;
    public int CurrentStamina
    {
        get { return _currentStamina;}
        set { _currentStamina = value;}
    }
}
