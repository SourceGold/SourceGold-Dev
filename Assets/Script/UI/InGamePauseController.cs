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
    private VisualElement rootBackground;
    private Label _title;

    private VisualElement _mainPageButtons;
    private SettingsPage _settings;
    private InputMap inputSystem;

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        var _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        var _settingButton = _doc.rootVisualElement.Q<Button>("SettingButton");
        var _exitButton = _doc.rootVisualElement.Q<Button>("ExitButton");

        _playButton.clicked += () => { 
            rootBackground.style.display = DisplayStyle.None; 
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        };
        _exitButton.clicked += () => Application.Quit();
        _settingButton.clicked += SettingsButtonOnClicked;

        rootBackground = _doc.rootVisualElement.Q<VisualElement>(name: "BlackBoxHolder");
        _displayArea = _doc.rootVisualElement.Q<VisualElement>(name: "BlackBox");
        _mainPageButtons = _doc.rootVisualElement.Q<VisualElement>(name: "Buttons");
        _title = _doc.rootVisualElement.Q<Label>(name: "HeaderLabel");
        _settings = new SettingsPage(ResetToPauseMenu, _settingFrame, _toggleBoxTemplate, _floatSliderTemplate, _intSliderTemplate, _enumFieldTemplate);

        rootBackground.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        // We make sure the cursor is invisible here
        // There might be a better place
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        inputSystem = FindObjectOfType<ControlManager>().InputMap;
        inputSystem.Player.EscClick.performed += EscOnClick;
    }

    public void EscOnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (rootBackground.style.display == DisplayStyle.None)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                rootBackground.style.display = DisplayStyle.Flex;
            } else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                rootBackground.style.display = DisplayStyle.None;
                _settings.forceExitSettings();
                ResetToPauseMenu();
            }
        }
    }

    private void SettingsButtonOnClicked()
    {
        rootBackground.RemoveFromClassList("in-game-pause-background");
        rootBackground.AddToClassList("setting-page-background");
        _title.text = "Settings";

        _settings.initializeSettings();
        _displayArea.Remove(_mainPageButtons);
        _displayArea.Add(_settings.getRootElement());

        
    }
    private void ResetToPauseMenu()
    {
        rootBackground.RemoveFromClassList("settingstarting-page-background");
        rootBackground.AddToClassList("in-game-pause-background");
        _title.text = "Game Pause";

        if (_displayArea.Contains(_settings.getRootElement()))
            _displayArea.Remove(_settings.getRootElement());
        _displayArea.Add(_mainPageButtons);
    }
}
