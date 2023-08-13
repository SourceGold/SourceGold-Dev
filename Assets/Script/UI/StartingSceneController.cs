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
    private VisualElement _settingsButtons;
    private ScrollView _settingsView;

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
    }

    private void Start()
    {
        _settingsButtons = _settingFrame.CloneTree();
        Length settingheight = new Length(100, LengthUnit.Percent);
        _settingsButtons.style.height = settingheight;

        _settingsView = _settingsButtons.Q<ScrollView>(name: "Scroll");
        _settingsButtons.Q<Button>("GamePlayButton").clicked += () => { _settingsView.Clear(); addGamePlayButtons(_settingsView); };
        _settingsButtons.Q<Button>("ControlButton").clicked += () => { _settingsView.Clear(); addControlButtons(_settingsView); };
        _settingsButtons.Q<Button>("GraphicsButton").clicked += () => { _settingsView.Clear(); addGraphicsButtons(_settingsView); };
        _settingsButtons.Q<Button>("AudioButton").clicked += () => { _settingsView.Clear(); addAudioButtons(_settingsView); };

        _settingsButtons.Q<Button>("SaveButton").clicked += () => { GlobalSettings.instance.SaveUserDefinedSettings();  };
        _settingsButtons.Q<Button>("SaveAndReturnButton").clicked += () => { GlobalSettings.instance.SaveUserDefinedSettings(); BackButtonOnClicked(); };
        _settingsButtons.Q<Button>("UndoAndReturnButton").clicked += () => { GlobalSettings.instance.LoadUserDefinedSettings(); BackButtonOnClicked(); };
        _settingsButtons.Q<Button>("ResetToDefaultButton").clicked += () => { 
            GlobalSettings.instance.LoadUserDefinedDefaultSettings(); 
            _settingsView.Clear(); 
            addGamePlayButtons(_settingsView); 
        };
    }

    private void SettingsButtonOnClicked()
    {
        _blackBox.RemoveFromClassList("starting-page-background");
        _blackBox.AddToClassList("setting-page-background");
        _title.text = "Settings";

        _displayArea.Remove(_mainPageButtons);
        _displayArea.Add(_settingsButtons);

        _settingsView.Clear();
        addGamePlayButtons(_settingsView);
    }
    private void BackButtonOnClicked()
    {
        _blackBox.RemoveFromClassList("settingstarting-page-background");
        _blackBox.AddToClassList("starting-page-background");
        _title.text = "Source Gold\nImpact";

        _displayArea.Remove(_settingsButtons);
        _displayArea.Add(_mainPageButtons);
    }

    private void addGamePlayButtons(ScrollView view)
    {
        setupEnumField(view, GlobalSettings.instance.userDefinedSettings.GamePlay.difficulties, "Difficulties")
            .RegisterValueChangedCallback(x => { GlobalSettings.instance.userDefinedSettings.GamePlay.difficulties = (difficulties)x.newValue; });
    }

    private void addControlButtons(ScrollView view)
    {
        setupToggleBox(view, GlobalSettings.instance.userDefinedSettings.Control.PressToSpeedUp, "Press to speed up")
            .RegisterValueChangedCallback(x => { GlobalSettings.instance.userDefinedSettings.Control.PressToSpeedUp = x.newValue; });

        setupToggleBox(view, GlobalSettings.instance.userDefinedSettings.Control.RevertCameraMovements, "Revert Camera Up and Down")
            .RegisterValueChangedCallback(x => {GlobalSettings.instance.userDefinedSettings.Control.RevertCameraMovements = x.newValue;});

        setupFloatSlider(view, GlobalSettings.instance.userDefinedSettings.Control.MouseSensitivity, "Mouse Sensitivity", 0.0f, 1.0f)
            .RegisterValueChangedCallback(x => { GlobalSettings.instance.userDefinedSettings.Control.MouseSensitivity = x.newValue; });
    }

    private void addGraphicsButtons(ScrollView view)
    {
        setupEnumField(view, GlobalSettings.instance.userDefinedSettings.Graphics.VisualQuality, "Visual quality")
            .RegisterValueChangedCallback(x => { GlobalSettings.instance.userDefinedSettings.Graphics.VisualQuality = (visualQuality)x.newValue; });
    }

    private void addAudioButtons(ScrollView view)
    {

    }

    private Toggle setupToggleBox(ScrollView view, bool _entry, string text)
    {
        var toggleBox = _toggleBoxTemplate.CloneTree();
        Toggle toggle = toggleBox.Q<Toggle>(name: "Toggle");
        toggle.value = _entry;
        var title = toggleBox.Q<Label>(name: "ToggleLabel");
        title.text = text;
        view.Add(toggleBox);
        return toggle;
    }

    private EnumField setupEnumField(ScrollView view, System.Enum _entry, string text)
    {
        var enumBox = _enumFieldTemplate.CloneTree();
        EnumField enumfield = enumBox.Q<EnumField>(name: "Selection");
        enumfield.Init(_entry);
        var title = enumBox.Q<Label>(name: "SelectionLabel");
        title.text = text;
        view.Add(enumBox);
        return enumfield;
    }

    private Slider setupFloatSlider(ScrollView view, float _entry, string text, float min, float max)
    {
        var toggleBox = _floatSliderTemplate.CloneTree();
        Slider toggle = toggleBox.Q<Slider>(name: "FloatSlider");
        var numberDisplay = toggleBox.Q<Label>(name: "FloatSliderNumber");
        numberDisplay.text = _entry.ToString("0.00");

        toggle.lowValue = min;
        toggle.highValue = max;
        toggle.value = GlobalSettings.instance.userDefinedSettings.Control.MouseSensitivity;
        toggle.RegisterValueChangedCallback(x =>
        {
            numberDisplay.text = GlobalSettings.instance.userDefinedSettings.Control.MouseSensitivity.ToString("0.00");
        });
        var title = toggleBox.Q<Label>(name: "FloatSliderLabel");
        title.text = text;
        view.Add(toggleBox);
        return toggle;
    }
    
    private Slider setupIntSlider(ScrollView view, int _entry, string text, int min, int max)
    {
        var toggleBox = _intSliderTemplate.CloneTree();
        Slider toggle = toggleBox.Q<Slider>(name: "IntSlider");
        var numberDisplay = toggleBox.Q<Label>(name: "IntSliderNumber");
        numberDisplay.text = _entry.ToString("0.00");

        toggle.lowValue = min;
        toggle.highValue = max;
        toggle.value = GlobalSettings.instance.userDefinedSettings.Control.MouseSensitivity;
        toggle.RegisterValueChangedCallback(x =>
        {
            numberDisplay.text = GlobalSettings.instance.userDefinedSettings.Control.MouseSensitivity.ToString();
        });
        var title = toggleBox.Q<Label>(name: "IntSliderLabel");
        title.text = text;
        view.Add(toggleBox);
        return toggle;
    }
}
