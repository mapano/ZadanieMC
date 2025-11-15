using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Selectable firstSelected;

    private GUIController _controller;

    public void Init(GUIController controller)
    {
        _controller = controller;

        if (playButton != null)
        {
            playButton.onClick.AddListener(() => _controller.HandlePlayRequested());
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() => _controller.HandleSettingsRequested());
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(() => _controller.HandleExitRequested());
        }

        Show(false);
    }

    public void Show(bool visible)
    {
        gameObject.SetActive(visible);

        if (visible && firstSelected != null)
        {
            EventSystem.current?.SetSelectedGameObject(firstSelected.gameObject);
        }
    }
}
