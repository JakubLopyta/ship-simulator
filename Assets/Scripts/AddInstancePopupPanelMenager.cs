using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelToggleTMP : MonoBehaviour
{
    public GameObject popupPanel;
    public Button openButton;
    public Button closeButton;

    void Start()
    {
        // Attach listeners
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);

        // Hide panel on start
        popupPanel.SetActive(false);
    }

    void OpenPanel()
    {
        popupPanel.SetActive(true);
    }

    void ClosePanel()
    {
        popupPanel.SetActive(false);
    }
}
