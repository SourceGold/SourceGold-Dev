using Assets.Script.Backend;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class MenuController : MonoBehaviour
{
    public SettingsPage _settings;
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
    

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        var _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        var _settingButton = _doc.rootVisualElement.Q<Button>("SettingButton");
        var _exitButton = _doc.rootVisualElement.Q<Button>("ExitButton");
        _playButton.clicked += () => SceneManager.LoadScene("Scenes/Scene1");
        _exitButton.clicked += () => Application.Quit();
        _settingButton.clicked += SettingsButtonOnClicked;

        _blackBox = _doc.rootVisualElement.Q<VisualElement>(name: "BlackBoxHolder");
        _displayArea = _doc.rootVisualElement.Q<VisualElement>(name: "BlackBox");
        _mainPageButtons = _doc.rootVisualElement.Q<VisualElement>(name: "Buttons");

        _title = _doc.rootVisualElement.Q<Label>(name: "HeaderLabel");

        _settings = new SettingsPage(BackButtonOnClicked, _settingFrame, _toggleBoxTemplate, _floatSliderTemplate, _intSliderTemplate, _enumFieldTemplate);
    }

    private void Start()
    {

    }

    private void SettingsButtonOnClicked()
    {
        _blackBox.RemoveFromClassList("starting-page-background");
        _blackBox.AddToClassList("setting-page-background");
        _title.text = "Settings";

        _settings.initializeSettings();
        _displayArea.Remove(_mainPageButtons);    
        _displayArea.Add(_settings.getRootElement());

        
    }
    private void BackButtonOnClicked()
    {
        _blackBox.RemoveFromClassList("settingstarting-page-background");
        _blackBox.AddToClassList("starting-page-background");
        _title.text = "Source Gold\nImpact";

        _displayArea.Remove(_settings.getRootElement());
        _displayArea.Add(_mainPageButtons);
    }
}
