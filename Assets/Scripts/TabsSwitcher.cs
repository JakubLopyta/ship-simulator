using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public string tabName;
        public Button button;
        public GameObject panel;
    }

    public Tab[] tabs;

    public string defaultTabName = "Instances";

    public Color selectedColor = Color.white;
    public Color32 defaultColor = new Color32(244, 244, 245, 255); // light gray


    void Start()
    {
        foreach (var tab in tabs)
        {
            tab.button.onClick.AddListener(() => OnTabSelected(tab));
        }

        // Set initial active tab
        if (tabs.Length > 0)
        {
            OnTabSelected(tabs[0]);
        }
    }

    void OnTabSelected(Tab selectedTab)
    {
        foreach (var tab in tabs)
        {
            bool isSelected = (tab == selectedTab);

            tab.panel.SetActive(isSelected);

            // Update the Button's color block
            var colors = tab.button.colors;
            colors.normalColor = isSelected ? selectedColor : defaultColor;
            colors.highlightedColor = isSelected ? selectedColor : defaultColor;
            colors.pressedColor = isSelected ? selectedColor : defaultColor;
            colors.selectedColor = isSelected ? selectedColor : defaultColor;
            colors.disabledColor = new Color(0.8f, 0.8f, 0.8f); // optional
            tab.button.colors = colors;
        }
    }
}
