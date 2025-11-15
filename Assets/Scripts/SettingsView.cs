using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Toggle checkboxOne;
    [SerializeField] private Toggle checkboxTwo;
    [SerializeField] private Button backButton;
    [SerializeField] private Selectable firstSelected;

    private GUIController _controller;

    public void Init(GUIController controller)
    {
        _controller = controller;

        if (backButton != null)
        {
            backButton.onClick.AddListener(() => _controller.HandleSettingsBackRequested());
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
