using UnityEngine;

public class GUIController : MonoBehaviour
{
    public static GUIController Instance { get; private set; }

    [SerializeField] private MainMenuView mainMenuView;
    [SerializeField] private SettingsView settingsView;
    [SerializeField] private bool startMenuVisible = false;

    private GameBootstrapper _bootstrapper;
    private InputService _inputService;
    private bool _menuVisible;

    private enum Panel
    {
        None,
        Main,
        Settings
    }

    private Panel _currentPanel = Panel.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        mainMenuView?.Init(this);
        settingsView?.Init(this);

        if (startMenuVisible)
        {
            ShowMainMenu();
        }
        else
        {
            HideMenu();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterBootstrapper(GameBootstrapper bootstrapper, InputService inputService)
    {
        _bootstrapper = bootstrapper;
        _inputService = inputService;
        _bootstrapper.SetPaused(_menuVisible);
        UpdateInputState();
    }

    private void Update()
    {
        if (_inputService == null || !_menuVisible)
        {
            return;
        }

        if (_inputService.EscapePressed)
        {
            if (_currentPanel == Panel.Settings)
            {
                HandleSettingsBackRequested();
            }
            else if (_currentPanel == Panel.Main)
            {
                HideMenu();
            }
            _inputService.ConsumeEscape();
        }
    }

    public void ToggleMenu()
    {
        if (_menuVisible)
        {
            HideMenu();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public void HandlePlayRequested()
    {
        _bootstrapper?.StartNewGame();
        HideMenu();
    }

    public void HandleSettingsRequested()
    {
        ShowSettings();
    }

    public void HandleExitRequested()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HandleSettingsBackRequested()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        _menuVisible = true;
        _currentPanel = Panel.Main;

        mainMenuView?.Show(true);
        settingsView?.Show(false);

        _bootstrapper?.SetPaused(true);
        UpdateInputState();
    }

    public void ShowSettings()
    {
        _menuVisible = true;
        _currentPanel = Panel.Settings;

        mainMenuView?.Show(false);
        settingsView?.Show(true);

        _bootstrapper?.SetPaused(true);
        UpdateInputState();
    }

    public void HideMenu()
    {
        _menuVisible = false;
        _currentPanel = Panel.None;

        mainMenuView?.Show(false);
        settingsView?.Show(false);

        _bootstrapper?.SetPaused(false);
        UpdateInputState();
    }

    private void UpdateInputState()
    {
        if (_inputService == null)
        {
            return;
        }

        switch (_currentPanel)
        {
            case Panel.Main:
                _inputService.SetState(GameState.MainMenu);
                break;
            case Panel.Settings:
                _inputService.SetState(GameState.Settings);
                break;
            default:
                _inputService.SetState(GameState.Gameplay);
                break;
        }
    }
}
