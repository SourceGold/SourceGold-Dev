using Assets.Script.Backend;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.EventTrigger;

public class InGamePauseController : MonoBehaviour
{
    [SerializeField]
    private VisualTreeAsset _settingFrame;
    [SerializeField]
    private VisualTreeAsset _toggleBoxTemplate;
    [SerializeField]
    private VisualTreeAsset _floatSliderTemplate;
    [SerializeField]
    private VisualTreeAsset _intSliderTemplate;
    [SerializeField]
    private VisualTreeAsset _enumFieldTemplate;

    private VisualElement _displayArea;
    private VisualElement _blackBox;
    private Label _title;

    private VisualElement _mainPageButtons;
    private SettingsPage _settings;
    private InputMap input;

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        var _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        var _settingButton = _doc.rootVisualElement.Q<Button>("SettingButton");
        var _exitButton = _doc.rootVisualElement.Q<Button>("ExitButton");
        _playButton.clicked += () => { 
            _blackBox.style.display = DisplayStyle.None; 
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        };
        _exitButton.clicked += () => Application.Quit();
        _settingButton.clicked += SettingsButtonOnClicked;

        _blackBox = _doc.rootVisualElement.Q<VisualElement>(name: "BlackBoxHolder");
        _displayArea = _doc.rootVisualElement.Q<VisualElement>(name: "BlackBox");
        _mainPageButtons = _doc.rootVisualElement.Q<VisualElement>(name: "Buttons");

        _title = _doc.rootVisualElement.Q<Label>(name: "HeaderLabel");
        _blackBox.style.display = DisplayStyle.None;
        _settings = new SettingsPage(BackButtonOnClicked, _settingFrame, _toggleBoxTemplate, _floatSliderTemplate, _intSliderTemplate, _enumFieldTemplate);
    }

    private void Start()
    {
        input = FindObjectOfType<ControlManager>().InputMap;
        input.Player.EscClick.performed += EscOnClick;
    }

    public void EscOnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_blackBox.style.display == DisplayStyle.None)
            {
                BackButtonOnClicked();
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                _blackBox.style.display = DisplayStyle.Flex;
            } else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                _blackBox.style.display = DisplayStyle.None;
            }
        }
    }

    private void SettingsButtonOnClicked()
    {
        _blackBox.RemoveFromClassList("in-game-background");
        _blackBox.AddToClassList("setting-page-background");
        _title.text = "Settings";

        _displayArea.Remove(_mainPageButtons);
        _displayArea.Add(_settings.initializeSettings());

        
    }
    private void BackButtonOnClicked()
    {
        _blackBox.RemoveFromClassList("settingstarting-page-background");
        _blackBox.AddToClassList("in-game-background");
        _title.text = "Game Pause";

        if (_displayArea.Contains(_settings.getVisualElement()))
            _displayArea.Remove(_settings.getVisualElement());
        _displayArea.Add(_mainPageButtons);
    }
}
