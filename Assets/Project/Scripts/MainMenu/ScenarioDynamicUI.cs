using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Pre-builds one CanvasGroup panel per scenario at startup.
/// Call BuildAll() once, then Show(index) to switch between them.
/// Attach to the Content GameObject inside the ScrollView.
/// </summary>
[ExecuteAlways]
public class ScenarioDynamicUI : MonoBehaviour
{
    // ── Style (tweak in Inspector) ────────────────────────────────────────────
    [Header("Colors")]
    [SerializeField] Color colorBackground  = Color.white;
    [SerializeField] Color colorSeparator   = new Color(0.878f, 0.878f, 0.878f);
    [SerializeField] Color colorText        = new Color(0.10f, 0.10f, 0.10f);
    [SerializeField] Color colorTextGray    = new Color(0.43f, 0.43f, 0.43f);
    [SerializeField] Color colorAccent      = new Color(0.184f, 0.502f, 0.929f);
    [SerializeField] Color colorInputBorder = new Color(0.878f, 0.878f, 0.878f);

    // Sizes — edit here to adjust; not serialized to avoid stale scene values
    const float fontSizeLabel   = 38f;
    const float fontSizeHeader  = 52f;
    const float fontSizeInput   = 38f;
    const float heightDropdown  = 100f;
    const float heightInput     = 100f;
    const float heightSliderRow = 120f;
    const float heightHandle    = 44f;

    // ── Runtime state ─────────────────────────────────────────────────────────
    CanvasGroup[] panels;
    Dictionary<string, System.Func<string>>[] gettersPerPanel;
    int activeIndex = -1;

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Creates one hidden CanvasGroup panel per scenario definition.</summary>
    public void BuildAll(ScenarioDefinition[] defs)
    {
        ClearChildren();

        panels = new CanvasGroup[defs.Length];
        gettersPerPanel = new Dictionary<string, System.Func<string>>[defs.Length];

        for (int i = 0; i < defs.Length; i++)
        {
            gettersPerPanel[i] = new Dictionary<string, System.Func<string>>();
            panels[i] = BuildPanel(defs[i], gettersPerPanel[i]);
        }

        // Hide all initially
        SetAllHidden();
    }

    /// <summary>Shows the panel at index, hides all others.</summary>
    public void Show(int index)
    {
        if (panels == null) return;

        for (int i = 0; i < panels.Length; i++)
        {
            bool visible = i == index;
            panels[i].alpha = visible ? 1f : 0f;
            panels[i].interactable = visible;
            panels[i].blocksRaycasts = visible;
        }
        activeIndex = index;
    }

    /// <summary>Returns current values from the active panel (key = parameter id).</summary>
    public Dictionary<string, string> CollectValues()
    {
        var result = new Dictionary<string, string>();
        if (gettersPerPanel == null || activeIndex < 0 || activeIndex >= gettersPerPanel.Length)
            return result;

        foreach (var kv in gettersPerPanel[activeIndex])
            result[kv.Key] = kv.Value();

        return result;
    }

    // ── Panel builder ─────────────────────────────────────────────────────────

    CanvasGroup BuildPanel(ScenarioDefinition def, Dictionary<string, System.Func<string>> getters)
    {
        // Container with CanvasGroup for show/hide
        var panelGO = new GameObject("Panel_" + def.scenarioId, typeof(RectTransform));
        panelGO.transform.SetParent(transform, false);

        var cg = panelGO.AddComponent<CanvasGroup>();

        // VerticalLayoutGroup so elements stack properly
        var vlg = panelGO.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(48, 48, 32, 32);
        vlg.spacing = 14;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        var csf = panelGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        if (def.parameters != null)
        {
            foreach (var param in def.parameters)
            {
                switch (param.type)
                {
                    case "header":   BuildHeader(panelGO,  param);          break;
                    case "dropdown": BuildDropdown(panelGO, param, getters); break;
                    case "slider":   BuildSlider(panelGO,   param, getters); break;
                    case "input":    BuildInput(panelGO,    param, getters); break;
                    case "row":      BuildRow(panelGO,      param, getters); break;
                    default:
                        Debug.LogWarning($"[ScenarioDynamicUI] Unknown type: '{param.type}'");
                        break;
                }
            }
        }

        return cg;
    }

    // ── Element builders ──────────────────────────────────────────────────────

    void BuildHeader(GameObject parent, ScenarioParameter param)
    {
        var titleGO = Child(parent, "Header_" + param.label);
        titleGO.AddComponent<LayoutElement>().preferredHeight = 38f;
        var t = titleGO.AddComponent<TextMeshProUGUI>();
        t.text = param.label; t.fontSize = fontSizeHeader;
        t.fontStyle = FontStyles.Bold; t.color = colorText; t.raycastTarget = false;

        var sep = ImgChild(parent, "Separator", colorSeparator);
        sep.AddComponent<LayoutElement>().preferredHeight = 1f;
    }

    void BuildDropdown(GameObject parent, ScenarioParameter param, Dictionary<string, System.Func<string>> getters)
    {
        Label(parent, param.label);

        var go = ImgChild(parent, "Dropdown_" + param.id, colorBackground);
        go.AddComponent<LayoutElement>().preferredHeight = heightDropdown;
        go.AddComponent<Outline>().effectColor = colorInputBorder;

        var dd = go.AddComponent<TMP_Dropdown>();
        dd.options.Clear();
        if (param.options != null)
            foreach (var opt in param.options)
                dd.options.Add(new TMP_Dropdown.OptionData(opt));

        var labelGO = Child(go, "Label");
        FillRT(labelGO, new Vector2(12, 0), new Vector2(-36, 0));
        var lt = labelGO.AddComponent<TextMeshProUGUI>();
        lt.fontSize = fontSizeInput; lt.color = colorText;
        lt.alignment = TextAlignmentOptions.MidlineLeft;
        if (param.options?.Length > 0) lt.text = param.options[0];
        dd.captionText = lt;

        var arrowGO = Child(go, "Arrow");
        var art = arrowGO.GetComponent<RectTransform>();
        art.anchorMin = new Vector2(1, 0.5f); art.anchorMax = new Vector2(1, 0.5f);
        art.pivot = new Vector2(1, 0.5f); art.anchoredPosition = new Vector2(-12, 0);
        art.sizeDelta = new Vector2(20, 20);
        var at = arrowGO.AddComponent<TextMeshProUGUI>();
        at.text = "v"; at.fontSize = fontSizeInput; at.color = colorTextGray;
        at.alignment = TextAlignmentOptions.Center;

        if (!string.IsNullOrEmpty(param.id))
            getters[param.id] = () => dd.options[dd.value].text;
    }

    void BuildSlider(GameObject parent, ScenarioParameter param, Dictionary<string, System.Func<string>> getters)
    {
        var row = Child(parent, "SliderRow_" + param.id);
        row.AddComponent<LayoutElement>().preferredHeight = heightSliderRow;
        var vlg = row.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8; vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

        var lGO = Child(row, "Label");
        lGO.AddComponent<LayoutElement>().preferredHeight = fontSizeLabel + 10f;
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.text = param.label; lt.fontSize = fontSizeLabel; lt.color = colorText; lt.raycastTarget = false;

        var svRow = Child(row, "SliderValueRow");
        svRow.AddComponent<LayoutElement>().preferredHeight = heightHandle + 12f;
        var hlg = svRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12; hlg.childControlWidth = true; hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false; hlg.childForceExpandHeight = true;

        var sGO = Child(svRow, "Slider");
        sGO.AddComponent<LayoutElement>().flexibleWidth = 1;
        var slider = sGO.AddComponent<Slider>();
        slider.minValue = param.min; slider.maxValue = param.max;
        slider.direction = Slider.Direction.LeftToRight;

        var bgGO = ImgChild(sGO, "Background", new Color(0.85f, 0.85f, 0.85f));
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.4f); bgRT.anchorMax = new Vector2(1, 0.6f);
        bgRT.sizeDelta = Vector2.zero; bgRT.anchoredPosition = Vector2.zero;

        var faGO = Child(sGO, "FillArea");
        var faRT = faGO.GetComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.4f); faRT.anchorMax = new Vector2(1, 0.6f);
        faRT.sizeDelta = Vector2.zero; faRT.anchoredPosition = Vector2.zero;
        faRT.offsetMax = new Vector2(-heightHandle * 0.5f, 0);
        faRT.offsetMin = new Vector2(heightHandle * 0.5f, 0);

        var fGO = ImgChild(faGO, "Fill", colorAccent);
        var fRT = fGO.GetComponent<RectTransform>();
        fRT.anchorMin = Vector2.zero; fRT.anchorMax = new Vector2(0, 1);
        fRT.sizeDelta = Vector2.zero; fRT.anchoredPosition = Vector2.zero;
        slider.fillRect = fRT;

        var haGO = Child(sGO, "HandleSlideArea");
        var haRT = haGO.GetComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero; haRT.anchorMax = Vector2.one;
        haRT.offsetMin = new Vector2(heightHandle * 0.5f, 0);
        haRT.offsetMax = new Vector2(-heightHandle * 0.5f, 0);

        var hGO = ImgChild(haGO, "Handle", colorAccent);
        var hRT = hGO.GetComponent<RectTransform>();
        hRT.anchorMin = new Vector2(0, 0.5f); hRT.anchorMax = new Vector2(0, 0.5f);
        hRT.pivot = new Vector2(0.5f, 0.5f);
        hRT.sizeDelta = new Vector2(heightHandle, heightHandle);
        hRT.anchoredPosition = Vector2.zero;
        slider.handleRect = hRT;
        slider.targetGraphic = hGO.GetComponent<Image>();

        // Set value AFTER fillRect/handleRect so Slider can position them correctly
        slider.SetValueWithoutNotify(param.defaultValue);

        var valGO = Child(svRow, "ValueLabel");
        valGO.AddComponent<LayoutElement>().preferredWidth = 90f;
        var valTMP = valGO.AddComponent<TextMeshProUGUI>();
        valTMP.fontSize = fontSizeLabel; valTMP.color = colorText;
        valTMP.alignment = TextAlignmentOptions.MidlineRight; valTMP.raycastTarget = false;
        valTMP.text = param.defaultValue.ToString("F1");
        slider.onValueChanged.AddListener(v => valTMP.text = v.ToString("F1"));

        if (!string.IsNullOrEmpty(param.id))
            getters[param.id] = () => slider.value.ToString("F2");
    }

    void BuildRow(GameObject parent, ScenarioParameter param, Dictionary<string, System.Func<string>> getters)
    {
        if (param.children == null || param.children.Length == 0) return;

        var row = Child(parent, "Row_" + param.id);
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 24; hlg.childControlWidth = true; hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true; hlg.childForceExpandHeight = false;
        row.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        foreach (var child in param.children)
        {
            var p = RowChildToParam(child);
            var col = Child(row, "Col_" + p.id);
            var colVlg = col.AddComponent<VerticalLayoutGroup>();
            colVlg.spacing = 4; colVlg.childControlWidth = true; colVlg.childControlHeight = true;
            colVlg.childForceExpandWidth = true; colVlg.childForceExpandHeight = false;
            col.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            switch (p.type)
            {
                case "slider":   BuildSlider(col,   p, getters); break;
                case "dropdown": BuildDropdown(col, p, getters); break;
                case "input":    BuildInput(col,    p, getters); break;
                default:
                    Debug.LogWarning($"[ScenarioDynamicUI] Unsupported type in row: '{p.type}'");
                    break;
            }
        }
    }

    static ScenarioParameter RowChildToParam(ScenarioRowChild c) => new ScenarioParameter
    {
        type = c.type, id = c.id, label = c.label, options = c.options,
        min = c.min, max = c.max, defaultValue = c.defaultValue, defaultText = c.defaultText
    };

    void BuildInput(GameObject parent, ScenarioParameter param, Dictionary<string, System.Func<string>> getters)
    {
        Label(parent, param.label);

        var go = ImgChild(parent, "Input_" + param.id, colorBackground);
        go.AddComponent<LayoutElement>().preferredHeight = heightInput;
        go.AddComponent<Outline>().effectColor = colorInputBorder;

        var inputField = go.AddComponent<TMP_InputField>();

        var textArea = Child(go, "TextArea");
        FillRT(textArea, new Vector2(12, 0), new Vector2(-12, 0));
        textArea.AddComponent<RectMask2D>();

        var textGO = Child(textArea, "Text");
        FillRT(textGO, Vector2.zero, Vector2.zero);
        var tTMP = textGO.AddComponent<TextMeshProUGUI>();
        tTMP.fontSize = fontSizeInput; tTMP.color = colorText;
        tTMP.alignment = TextAlignmentOptions.MidlineLeft;

        inputField.textComponent = tTMP;
        inputField.textViewport = textArea.GetComponent<RectTransform>();
        inputField.text = param.defaultText ?? "";

        if (!string.IsNullOrEmpty(param.id))
            getters[param.id] = () => inputField.text;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    void Label(GameObject parent, string text)
    {
        var go = Child(parent, "Label_" + text.Replace(" ", ""));
        go.AddComponent<LayoutElement>().preferredHeight = 26f;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = fontSizeLabel; t.color = colorText; t.raycastTarget = false;
    }

    void SetAllHidden()
    {
        if (panels == null) return;
        foreach (var cg in panels)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }
    }

    GameObject Child(GameObject parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        return go;
    }

    GameObject ImgChild(GameObject parent, string name, Color color)
    {
        var go = Child(parent, name);
        go.AddComponent<Image>().color = color;
        return go;
    }

    static void FillRT(GameObject go, Vector2 offsetMin, Vector2 offsetMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }
}
