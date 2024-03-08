using Assets.Script;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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

    private ControlManager _controlManager;

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        var _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        var _settingButton = _doc.rootVisualElement.Q<Button>("SettingButton");
        var _exitButton = _doc.rootVisualElement.Q<Button>("ExitButton");
        var _saveButton = _doc.rootVisualElement.Q<Button>("SaveButton");
        var _loadButton = _doc.rootVisualElement.Q<Button>("LoadButton");
        var _restartButton = _doc.rootVisualElement.Q<Button>("RestartButton");

        _controlManager = FindObjectOfType<ControlManager>();
        _playButton.clicked += () =>
        {
            rootBackground.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            _controlManager.ToggleInputActionMap();
        };
        _exitButton.clicked += () => Application.Quit();
        _settingButton.clicked += SettingsButtonOnClicked;
        _saveButton.clicked += () => DataPersistenceManager.SaveGame(DataPersistenceManager._testSave);
        _loadButton.clicked += () => DataPersistenceManager.LoadGame(DataPersistenceManager._testSave);
        _restartButton.clicked += () => DataPersistenceManager.RestartGame();


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
    }

    public void EscOnClick()
    {
        if (rootBackground.style.display == DisplayStyle.None)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            rootBackground.style.display = DisplayStyle.Flex;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            rootBackground.style.display = DisplayStyle.None;
            _settings.forceExitSettings();
            ResetToPauseMenu();
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
