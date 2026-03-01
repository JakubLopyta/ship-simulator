using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsPanelSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        [SerializeField] private string tabName;
        [SerializeField] private Button button;
        [SerializeField] private GameObject panel;

        public string TabName => tabName;
        public Button Button => button;
        public GameObject Panel => panel;
    }

    [SerializeField] private Tab[] tabs;

    [SerializeField] private Color32 selectedColor = new(78, 101, 192, 190);
    [SerializeField] private Color32 defaultColor = new(78, 101, 192, 0);

    private UnityAction[] tabListeners;

    void Start()
    {
        tabListeners = new UnityAction[tabs.Length];

        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            var button = tabs[index].Button;
            if (button == null) continue;

            UnityAction listener = () => OnTabSelected(tabs[index]);
            button.onClick.AddListener(listener);
            tabListeners[index] = listener;
        }

        // Set initial active tab
        if (tabs.Length > 0)
        {
            OnTabSelected(tabs[0]);
        }
    }

    void OnDestroy()
    {
        if (tabs == null || tabListeners == null) return;

        int count = Mathf.Min(tabs.Length, tabListeners.Length);
        for (int i = 0; i < count; i++)
        {
            var button = tabs[i].Button;
            var listener = tabListeners[i];
            if (button == null || listener == null) continue;

            button.onClick.RemoveListener(listener);
        }
    }

    void OnTabSelected(Tab selectedTab)
    {
        foreach (var tab in tabs)
        {
            bool isSelected = tab == selectedTab;

            if (tab.Panel != null)
            {
                tab.Panel.SetActive(isSelected);
            }

            // Update the Button's color block
            if (tab.Button == null) continue;

            var colors = tab.Button.colors;
            colors.normalColor = isSelected ? selectedColor : defaultColor;
            colors.selectedColor = isSelected ? selectedColor : defaultColor;
            tab.Button.colors = colors;
        }
    }
}
