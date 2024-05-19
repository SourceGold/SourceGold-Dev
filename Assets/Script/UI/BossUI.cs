using Assets.Script.Backend;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BossUI : MonoBehaviour
{
    Label _HealthLabel;
    VisualElement _HealthBar;
    VisualElement _rootElement;

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        _rootElement = _doc.rootVisualElement;
        _HealthBar = _doc.rootVisualElement.Q<VisualElement>("HealthBar");
        _HealthLabel = _doc.rootVisualElement.Q<Label>("HealthLabel");
        setActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Register
        // Backend.GameLoop.RegisterPlayerOnStatsChangeCallBack(PlayerStatsChangeCallback);
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
    }

    public void setActive(bool activated)
    {
        _rootElement.style.visibility = activated ? Visibility.Visible : Visibility.Hidden;
    }

}
