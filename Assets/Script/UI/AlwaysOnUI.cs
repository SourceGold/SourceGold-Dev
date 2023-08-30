using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AlwaysOnUI : MonoBehaviour
{
    Label _HealthLabel;
    Label _ManaLabel;
    Label _StaminaLabel;

    VisualElement _HealthBar;
    VisualElement _ManaBar;
    VisualElement _StaminaBar;

    VisualElement _StatusHolder;
    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        _HealthBar = _doc.rootVisualElement.Q<VisualElement>("HealthBar");
        _ManaBar = _doc.rootVisualElement.Q<VisualElement>("ManaBar");
        _StaminaBar = _doc.rootVisualElement.Q<VisualElement>("StaminaBar");

        _HealthLabel = _doc.rootVisualElement.Q<Label>("HealthLabel");
        _ManaLabel = _doc.rootVisualElement.Q<Label>("ManaLabel");
        _StaminaLabel = _doc.rootVisualElement.Q<Label>("StaminaLabel");

        _StatusHolder = _doc.rootVisualElement.Q<VisualElement>("StatusHolder");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayerStatsChangeCallback(PlayableCharacterStats newStats)
    {
        _HealthLabel.text = newStats.CurrentHitPoint.ToString() + "/" + newStats.MaxHitPoint.ToString();
        int healthPercent = 100 - (int)((float)newStats.CurrentHitPoint / (float)newStats.MaxHitPoint * 100.0);
        healthPercent = Mathf.Clamp(healthPercent, 0, 100);
        _HealthBar.style.marginRight = new Length(healthPercent, LengthUnit.Percent);

        _ManaLabel.text = newStats.CurrentMagicPoint.ToString() + "/" + newStats.MaxMagicPoint.ToString();
        int manaPercent = 100 - (int)((float)newStats.CurrentMagicPoint / (float)newStats.MaxMagicPoint * 100.0);
        manaPercent = Mathf.Clamp(manaPercent, 0, 100);
        _ManaBar.style.marginRight = new Length(manaPercent, LengthUnit.Percent);

        _StaminaLabel.text = newStats.CurrentStamina.ToString() + "/" + newStats.MaxStamina.ToString();
        int staminaPercent = 100 - (int)((float)newStats.CurrentStamina / (float)newStats.MaxStamina * 100.0);
        staminaPercent = Mathf.Clamp(staminaPercent, 0, 100);
        _StaminaBar.style.marginRight = new Length(staminaPercent, LengthUnit.Percent);
        PlayerBuffChangeCallback(newStats);
    }

    void PlayerBuffChangeCallback(PlayableCharacterStats newStats)
    {
        //VisualElement Icon = new VisualElement();
        //Icon.AddToClassList("status-icon");
        //Icon.style.backgroundImage = Resources.Load<Texture2D>("Icons/PlayerStatus/healing");
        //_StatusHolder.Add(Icon);
    }

}
