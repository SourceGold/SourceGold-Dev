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
        _HealthLabel.text = ((int)newStats.CurrentHp).ToString() + "/" + ((int)newStats.MaxHitPoint).ToString();
        int healthPercent = 100 - (int)((float)newStats.CurrentHp / (float)newStats.MaxHitPoint * 100.0);
        healthPercent = Mathf.Clamp(healthPercent, 0, 100);
        _HealthBar.style.marginRight = new Length(healthPercent, LengthUnit.Percent);


        PlayerBuffChangeCallback(newStats);
    }

    void PlayerBuffChangeCallback(PlayableCharacterStats newStats)
    {
        //VisualElement Icon = new VisualElement();
        //Icon.AddToClassList("status-icon");
        //var texture = Resources.Load<Texture2D>("Icons/PlayerStatus/healing");
        //Icon.style.backgroundImage = texture;
        //_StatusHolder.Add(Icon);
    }
    
}
