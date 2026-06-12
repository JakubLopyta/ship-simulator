using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Loads all scenario JSON files and populates the left-panel button list.
/// Attach to the LeftPanel GameObject.
/// </summary>
[ExecuteAlways]
public class ScenarioListUI : MonoBehaviour
{
    [SerializeField] ScenarioDynamicUI dynamicUI;

    [Header("Button Style")]
    [SerializeField] Color colorDefault      = new Color(0, 0, 0, 0);
    [SerializeField] Color colorSelected     = new Color(0.184f, 0.502f, 0.929f);
    [SerializeField] Color colorTextDefault  = new Color(0.10f, 0.10f, 0.10f);
    [SerializeField] Color colorTextSelected = Color.white;
    // Sizes — edit here; not serialized to avoid stale scene values
    const float buttonHeight = 136f;
    const float fontSize     = 44f;

    ScenarioDefinition[] scenarios;
    Button[] buttons;
    int selectedIndex = -1;

    void OnEnable()
    {
        // Auto-find if not set in Inspector
        if (dynamicUI == null)
            dynamicUI = FindAnyObjectByType<ScenarioDynamicUI>();

        scenarios = ScenarioLoader.LoadAll();
        if (scenarios.Length == 0)
        {
            Debug.LogWarning("[ScenarioListUI] No scenarios found in Resources/Scenarios/");
            return;
        }

        dynamicUI?.BuildAll(scenarios);
        SpawnButtons();
        SelectScenario(0);
    }

    void SpawnButtons()
    {
        // Remove any static placeholder buttons that may exist from editor setup
        ClearDynamicButtons();

        buttons = new Button[scenarios.Length];

        for (int i = 0; i < scenarios.Length; i++)
        {
            int index = i; // capture for lambda
            var def = scenarios[i];

            var go = new GameObject("ScenarioBtn_" + def.scenarioId, typeof(RectTransform));
            go.transform.SetParent(transform, false);

            go.AddComponent<LayoutElement>().preferredHeight = buttonHeight;
            var img = go.AddComponent<Image>();
            img.color = colorDefault;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => SelectScenario(index));
            buttons[i] = btn;

            // Left accent bar (shown when selected)
            var accent = new GameObject("Accent", typeof(RectTransform));
            accent.transform.SetParent(go.transform, false);
            var art = accent.GetComponent<RectTransform>();
            art.anchorMin = Vector2.zero; art.anchorMax = new Vector2(0, 1);
            art.sizeDelta = new Vector2(5, 0); art.anchoredPosition = Vector2.zero;
            accent.AddComponent<Image>().color = Color.white;
            accent.SetActive(false);

            // Label
            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(go.transform, false);
            var lRT = labelGO.GetComponent<RectTransform>();
            lRT.anchorMin = Vector2.zero; lRT.anchorMax = Vector2.one;
            lRT.offsetMin = new Vector2(20, 0); lRT.offsetMax = Vector2.zero;
            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text = def.title;
            tmp.fontSize = fontSize;
            tmp.color = colorTextDefault;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false;
        }
    }

    void SelectScenario(int index)
    {
        if (scenarios == null || index < 0 || index >= scenarios.Length) return;

        // Update button visuals
        for (int i = 0; i < buttons.Length; i++)
        {
            bool sel = i == index;
            var img = buttons[i].GetComponent<Image>();
            img.color = sel ? colorSelected : colorDefault;

            var accent = buttons[i].transform.Find("Accent");
            if (accent) accent.gameObject.SetActive(sel);

            var label = buttons[i].transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (label) label.color = sel ? colorTextSelected : colorTextDefault;

            var lRT = buttons[i].transform.Find("Label")?.GetComponent<RectTransform>();
            if (lRT) lRT.offsetMin = new Vector2(sel ? 22 : 20, 0);
        }

        selectedIndex = index;
        dynamicUI?.Show(index);
    }

    void ClearDynamicButtons()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (!child.name.StartsWith("ScenarioBtn_")) continue;
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }

    /// <summary>Returns the currently selected scenario definition.</summary>
    public ScenarioDefinition GetSelected() =>
        (scenarios != null && selectedIndex >= 0) ? scenarios[selectedIndex] : null;
}
