using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class ScenariosUISetup
{
    static readonly Color BgGray      = new Color(0.918f, 0.918f, 0.918f);
    static readonly Color White       = Color.white;
    static readonly Color SidebarBg   = new Color(0.973f, 0.973f, 0.973f);
    static readonly Color BlueAccent  = new Color(0.184f, 0.502f, 0.929f);
    static readonly Color InfoGray    = new Color(0.945f, 0.945f, 0.945f);
    static readonly Color LineGray    = new Color(0.878f, 0.878f, 0.878f);
    static readonly Color Black       = new Color(0.10f, 0.10f, 0.10f);
    static readonly Color Gray        = new Color(0.43f, 0.43f, 0.43f);
    static readonly Color WipOrange   = new Color(0.95f, 0.60f, 0.29f);
    static readonly Color Clear       = new Color(0, 0, 0, 0);

    const float LeftFrac   = 0.20f;   // left sidebar: 20% of canvas width
    const float FooterFrac = 0.08f;   // footer: 8% of canvas height

    [MenuItem("Ship Simulator/Setup Scenarios UI")]
    public static void CreateScenariosUI()
    {
        var scenarios = GameObject.Find("Scenarios");
        if (scenarios == null) { Debug.LogError("'Scenarios' not found!"); return; }

        while (scenarios.transform.childCount > 0)
            Object.DestroyImmediate(scenarios.transform.GetChild(0).gameObject);

        var si = scenarios.GetComponent<Image>();
        if (si) si.color = BgGray;

        // ──────────────────────────────────────────────────────────────────────
        // LEFT SIDEBAR
        // ──────────────────────────────────────────────────────────────────────
        var left = Img(scenarios, "LeftPanel", SidebarBg);
        Stretch(left, 0, FooterFrac, LeftFrac, 1);

        var leftVLG = left.AddComponent<VerticalLayoutGroup>();
        leftVLG.childControlWidth  = true;
        leftVLG.childControlHeight = true;   // VLG drives child heights via LayoutElement
        leftVLG.childForceExpandWidth  = true;
        leftVLG.childForceExpandHeight = false;
        leftVLG.spacing = 0;
        leftVLG.padding = new RectOffset(0, 0, 0, 0);

        // "Scenarios" header
        var sideHeader = Child(left, "ScenariosHeader");
        sideHeader.AddComponent<LayoutElement>().preferredHeight = 90;
        var ht = sideHeader.AddComponent<TextMeshProUGUI>();
        ht.text = "Scenarios"; ht.fontSize = 26; ht.fontStyle = FontStyles.Bold;
        ht.color = Black; ht.margin = new Vector4(20, 0, 0, 0);
        ht.alignment = TextAlignmentOptions.MidlineLeft; ht.raycastTarget = false;

        HLine(left, 1);

        ScenarioBtn(left, "OpenWaterFreeRoam", "Open Water Free Roam", false);
        ScenarioBtn(left, "TargetShipPassing",  "Target Ship Passing",   true);
        ScenarioBtn(left, "ManoeuvringTrials",  "Manoeuvring Trials",    false);

        // Vertical divider between panels
        var div = Img(scenarios, "Divider", LineGray);
        Stretch(div, LeftFrac, FooterFrac, LeftFrac, 1);
        div.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 0);

        // ──────────────────────────────────────────────────────────────────────
        // RIGHT PANEL  (scroll view)
        // ──────────────────────────────────────────────────────────────────────
        var right = Img(scenarios, "RightPanel", White);
        Stretch(right, LeftFrac, FooterFrac, 1, 1);

        // ScrollRect
        var scrollGO = Child(right, "ScrollView");
        Stretch(scrollGO, 0, 0, 1, 1);
        scrollGO.AddComponent<Image>().color = White;
        var scroll = scrollGO.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.scrollSensitivity = 50;
        scrollGO.AddComponent<RectMask2D>();

        // Content container (grows downward)
        var content = Child(scrollGO, "Content");
        var cRT = content.GetComponent<RectTransform>();
        cRT.anchorMin = new Vector2(0, 1);
        cRT.anchorMax = new Vector2(1, 1);
        cRT.pivot     = new Vector2(0.5f, 1);
        cRT.sizeDelta = Vector2.zero;
        cRT.anchoredPosition = Vector2.zero;

        var cVLG = content.AddComponent<VerticalLayoutGroup>();
        cVLG.padding = new RectOffset(48, 48, 36, 36);
        cVLG.spacing = 28;
        cVLG.childControlWidth  = true;
        cVLG.childControlHeight = true;
        cVLG.childForceExpandWidth  = true;
        cVLG.childForceExpandHeight = false;

        var cCSF = content.AddComponent<ContentSizeFitter>();
        cCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content = cRT;

        // ── Own Vessel ─────────────────────────────────────────────────────
        var s1 = Section(content, "OwnVesselSection");
        SectionHeader(s1, "Own Vessel");
        Label(s1, "Select Ship Class", 13, Black);
        Dropdown(s1, new[] { "cargo_general", "tanker_lng" });
        InfoBar(s1, "Displacement: 24,500 t   |   Length: 185 m   |   Max Speed: 21.5 kn");

        // ── Initial Conditions ──────────────────────────────────────────────
        var s2 = Section(content, "InitialConditionsSection");
        SectionHeader(s2, "Initial Conditions");
        SliderRow(s2, "SpeedRow", "Speed (knots)", 0, 25, 14);
        InputRow(s2, "HeadingRow", "Heading (degrees)", "045.0");

        // ── Environment ─────────────────────────────────────────────────────
        var s3 = Section(content, "EnvironmentSection");
        SectionHeader(s3, "Environment");
        TimeOfDayRow(s3);
        Label(s3, "Weather Preset", 13, Black);
        Dropdown(s3, new[] { "Clear", "Fog", "Rain", "Thunderstorm" });
        WipRow(s3, "Wind Speed");
        WipRow(s3, "Wave Height");

        // ── Scenario Settings ────────────────────────────────────────────────
        var s4 = Section(content, "ScenarioSettingsSection");
        SectionHeader(s4, "Scenario Settings");
        Label(s4, "Obstacle Ship Type", 13, Black);
        Dropdown(s4, new[] { "tanker_lng", "cargo_general" });

        var paramsRow = Child(s4, "ObstacleParamsRow");
        paramsRow.AddComponent<LayoutElement>().preferredHeight = 80;
        var pHLG = paramsRow.AddComponent<HorizontalLayoutGroup>();
        pHLG.spacing = 32;
        pHLG.childControlWidth  = true;
        pHLG.childControlHeight = true;
        pHLG.childForceExpandWidth  = true;
        pHLG.childForceExpandHeight = true;

        LabeledInput(paramsRow, "ObstacleSpeed",   "Obstacle Speed (knots)",    "10.5");
        LabeledInput(paramsRow, "ObstacleHeading", "Obstacle Heading (degrees)", "270.0");

        // ──────────────────────────────────────────────────────────────────────
        // FOOTER
        // ──────────────────────────────────────────────────────────────────────
        var footer = Img(scenarios, "Footer", White);
        Stretch(footer, LeftFrac, 0, 1, FooterFrac);

        // top border line
        var fb = Img(footer, "FooterBorder", LineGray);
        fb.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        fb.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        fb.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1);

        Btn(footer, "BackButton",  "Back to Main Menu", White, Black, true,
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(48, 0), new Vector2(220, 52));
        Btn(footer, "StartButton", "Start Simulation", BlueAccent, White, false,
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-48, 0), new Vector2(220, 52));

        var footerLeft = Img(scenarios, "FooterLeft", SidebarBg);
        Stretch(footerLeft, 0, 0, LeftFrac, FooterFrac);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("[ScenariosUISetup] Done!");
    }

    // ── LAYOUT SECTION ────────────────────────────────────────────────────────

    static GameObject Section(GameObject parent, string name)
    {
        var go = Child(parent, name);
        var vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
        vlg.childControlWidth  = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        var csf = go.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return go;
    }

    static void SectionHeader(GameObject parent, string title)
    {
        var go = Child(parent, "SectionTitle");
        go.AddComponent<LayoutElement>().preferredHeight = 38;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = title; t.fontSize = 22; t.fontStyle = FontStyles.Bold;
        t.color = Black; t.raycastTarget = false;
        HLine(parent, 1);
    }

    static void HLine(GameObject parent, float height)
    {
        var go = Img(parent, "HLine", LineGray);
        go.AddComponent<LayoutElement>().preferredHeight = height;
    }

    static void ScenarioBtn(GameObject parent, string name, string label, bool selected)
    {
        var go = Img(parent, name, selected ? BlueAccent : Clear);
        go.AddComponent<Button>();
        go.AddComponent<LayoutElement>().preferredHeight = 68;

        if (selected)
        {
            var accent = Img(go, "Accent", White);
            var art = accent.GetComponent<RectTransform>();
            art.anchorMin = Vector2.zero; art.anchorMax = new Vector2(0, 1);
            art.sizeDelta = new Vector2(5, 0); art.anchoredPosition = Vector2.zero;
        }

        var tGO = Child(go, "Label");
        var tRT = tGO.GetComponent<RectTransform>();
        tRT.anchorMin = Vector2.zero; tRT.anchorMax = Vector2.one;
        tRT.offsetMin = new Vector2(selected ? 22 : 20, 0);
        tRT.offsetMax = Vector2.zero;
        var tmp = tGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.fontSize = 18;
        tmp.color = selected ? White : Black;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.raycastTarget = false;
    }

    static void Label(GameObject parent, string text, float size, Color color)
    {
        var go = Child(parent, "Label_" + text.Replace(" ", ""));
        go.AddComponent<LayoutElement>().preferredHeight = 26;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = size; t.color = color; t.raycastTarget = false;
    }

    static void InfoBar(GameObject parent, string text)
    {
        var go = Img(parent, "ShipInfoBar", InfoGray);
        go.AddComponent<LayoutElement>().preferredHeight = 52;
        var tGO = Child(go, "Text");
        FillRT(tGO, new Vector2(16, 0), new Vector2(-16, 0));
        var t = tGO.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = 12; t.color = Gray;
        t.alignment = TextAlignmentOptions.MidlineLeft;
        t.raycastTarget = false;
    }

    static void Dropdown(GameObject parent, string[] options)
    {
        var go = Img(parent, "Dropdown", White);
        go.AddComponent<LayoutElement>().preferredHeight = 52;
        go.AddComponent<Outline>().effectColor = LineGray;
        var dd = go.AddComponent<TMP_Dropdown>();
        dd.options.Clear();
        foreach (var o in options) dd.options.Add(new TMP_Dropdown.OptionData(o));

        var lGO = Child(go, "Label");
        FillRT(lGO, new Vector2(12, 0), new Vector2(-36, 0));
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.fontSize = 16; lt.color = Black;
        lt.alignment = TextAlignmentOptions.MidlineLeft;
        if (options.Length > 0) lt.text = options[0];
        dd.captionText = lt;

        var aGO = Child(go, "Arrow");
        var aRT = aGO.GetComponent<RectTransform>();
        aRT.anchorMin = new Vector2(1, 0.5f); aRT.anchorMax = new Vector2(1, 0.5f);
        aRT.pivot = new Vector2(1, 0.5f); aRT.anchoredPosition = new Vector2(-12, 0);
        aRT.sizeDelta = new Vector2(20, 20);
        var at = aGO.AddComponent<TextMeshProUGUI>();
        at.text = "v"; at.fontSize = 16; at.color = Gray;
        at.alignment = TextAlignmentOptions.Center;
    }

    static void SliderRow(GameObject parent, string name, string labelText, float min, float max, float val)
    {
        var row = Child(parent, name);
        row.AddComponent<LayoutElement>().preferredHeight = 86;
        var vlg = row.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

        var lGO = Child(row, "Label");
        lGO.AddComponent<LayoutElement>().preferredHeight = 26;
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.text = labelText; lt.fontSize = 16; lt.color = Black; lt.raycastTarget = false;

        var svRow = Child(row, "SliderValueRow");
        svRow.AddComponent<LayoutElement>().preferredHeight = 44;
        var hlg = svRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12;
        hlg.childControlWidth = true; hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false; hlg.childForceExpandHeight = true;

        // Slider
        var sGO = Child(svRow, "Slider");
        sGO.AddComponent<LayoutElement>().flexibleWidth = 1;
        var slider = sGO.AddComponent<Slider>();
        slider.minValue = min; slider.maxValue = max; slider.value = val;
        slider.direction = Slider.Direction.LeftToRight;

        var bgGO = Img(sGO, "Background", new Color(0.85f, 0.85f, 0.85f));
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.35f); bgRT.anchorMax = new Vector2(1, 0.65f);
        bgRT.sizeDelta = Vector2.zero;

        var faGO = Child(sGO, "FillArea");
        var faRT = faGO.GetComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.35f); faRT.anchorMax = new Vector2(1, 0.65f);
        faRT.offsetMin = Vector2.zero; faRT.offsetMax = new Vector2(-8, 0);

        var fGO = Img(faGO, "Fill", BlueAccent);
        var fRT = fGO.GetComponent<RectTransform>();
        fRT.anchorMin = Vector2.zero; fRT.anchorMax = new Vector2(0, 1); fRT.sizeDelta = Vector2.zero;
        slider.fillRect = fRT;

        var haGO = Child(sGO, "HandleSlideArea");
        var haRT = haGO.GetComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero; haRT.anchorMax = Vector2.one;
        haRT.offsetMin = new Vector2(8, 0); haRT.offsetMax = new Vector2(-8, 0);

        var hGO = Img(haGO, "Handle", BlueAccent);
        hGO.GetComponent<RectTransform>().sizeDelta = new Vector2(22, 22);
        slider.handleRect = hGO.GetComponent<RectTransform>();
        slider.targetGraphic = hGO.GetComponent<Image>();

        // Input
        var iGO = Img(svRow, "ValueInput", White);
        iGO.AddComponent<LayoutElement>().preferredWidth = 90;
        iGO.AddComponent<Outline>().effectColor = LineGray;
        InputField(iGO, val.ToString("F1"));
    }

    static void InputRow(GameObject parent, string name, string labelText, string defaultVal)
    {
        var row = Child(parent, name);
        row.AddComponent<LayoutElement>().preferredHeight = 86;
        var vlg = row.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

        var lGO = Child(row, "Label");
        lGO.AddComponent<LayoutElement>().preferredHeight = 26;
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.text = labelText; lt.fontSize = 16; lt.color = Black; lt.raycastTarget = false;

        var iGO = Img(row, "Input", White);
        iGO.AddComponent<LayoutElement>().preferredHeight = 44;
        iGO.AddComponent<Outline>().effectColor = LineGray;
        InputField(iGO, defaultVal);
    }

    static void TimeOfDayRow(GameObject parent)
    {
        var row = Child(parent, "TimeOfDayRow");
        row.AddComponent<LayoutElement>().preferredHeight = 86;
        var vlg = row.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

        var lGO = Child(row, "Label");
        lGO.AddComponent<LayoutElement>().preferredHeight = 22;
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.text = "Time of Day"; lt.fontSize = 13; lt.color = Black; lt.raycastTarget = false;

        var svRow = Child(row, "SliderTimeRow");
        svRow.AddComponent<LayoutElement>().preferredHeight = 44;
        var hlg = svRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12;
        hlg.childControlWidth = true; hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false; hlg.childForceExpandHeight = true;

        var sGO = Child(svRow, "TimeSlider");
        sGO.AddComponent<LayoutElement>().flexibleWidth = 1;
        var slider = sGO.AddComponent<Slider>();
        slider.minValue = 0; slider.maxValue = 1439; slider.value = 435;
        slider.direction = Slider.Direction.LeftToRight;

        var bgGO = Img(sGO, "Background", new Color(0.85f, 0.85f, 0.85f));
        bgGO.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.35f);
        bgGO.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.65f);
        bgGO.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        var faGO = Child(sGO, "FillArea");
        faGO.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.35f);
        faGO.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.65f);
        faGO.GetComponent<RectTransform>().offsetMax = new Vector2(-8, 0);
        var fGO = Img(faGO, "Fill", BlueAccent);
        fGO.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        fGO.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        slider.fillRect = fGO.GetComponent<RectTransform>();

        var haGO = Child(sGO, "HandleSlideArea");
        haGO.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        haGO.GetComponent<RectTransform>().anchorMax = Vector2.one;
        haGO.GetComponent<RectTransform>().offsetMin = new Vector2(8, 0);
        haGO.GetComponent<RectTransform>().offsetMax = new Vector2(-8, 0);
        var hGO = Img(haGO, "Handle", BlueAccent);
        hGO.GetComponent<RectTransform>().sizeDelta = new Vector2(22, 22);
        slider.handleRect = hGO.GetComponent<RectTransform>();
        slider.targetGraphic = hGO.GetComponent<Image>();

        var tGO = Child(svRow, "TimeDisplay");
        tGO.AddComponent<LayoutElement>().preferredWidth = 55;
        var tt = tGO.AddComponent<TextMeshProUGUI>();
        tt.text = "07:15"; tt.fontSize = 13; tt.color = Black;
        tt.alignment = TextAlignmentOptions.MidlineRight; tt.raycastTarget = false;
    }

    static void WipRow(GameObject parent, string labelText)
    {
        var row = Child(parent, "Wip_" + labelText.Replace(" ", ""));
        row.AddComponent<LayoutElement>().preferredHeight = 52;
        var vlg = row.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8;
        vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;
        row.AddComponent<CanvasGroup>().alpha = 0.45f;

        // Label + WIP tag in one row
        var labelRow = Child(row, "LabelRow");
        labelRow.AddComponent<LayoutElement>().preferredHeight = 22;
        var hlg = labelRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childControlWidth = false; hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false; hlg.childForceExpandHeight = true;

        var lGO = Child(labelRow, "Label");
        lGO.AddComponent<LayoutElement>().preferredWidth = 110;
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.text = labelText; lt.fontSize = 13; lt.color = Gray; lt.raycastTarget = false;

        var wGO = Child(labelRow, "WipTag");
        wGO.AddComponent<LayoutElement>().preferredWidth = 38;
        var wt = wGO.AddComponent<TextMeshProUGUI>();
        wt.text = "WIP"; wt.fontSize = 11; wt.fontStyle = FontStyles.Bold;
        wt.color = WipOrange; wt.raycastTarget = false;

        // Track only (no interactive slider)
        var track = Img(row, "Track", new Color(0.85f, 0.85f, 0.85f));
        track.AddComponent<LayoutElement>().preferredHeight = 6;
    }

    static void LabeledInput(GameObject parent, string name, string labelText, string defaultVal)
    {
        var group = Child(parent, name);
        var vlg = group.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childControlWidth = true; vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

        var lGO = Child(group, "Label");
        lGO.AddComponent<LayoutElement>().preferredHeight = 20;
        var lt = lGO.AddComponent<TextMeshProUGUI>();
        lt.text = labelText; lt.fontSize = 12; lt.color = Black; lt.raycastTarget = false;

        var iGO = Img(group, "Input", White);
        iGO.AddComponent<LayoutElement>().preferredHeight = 44;
        iGO.AddComponent<Outline>().effectColor = LineGray;
        InputField(iGO, defaultVal);
    }

    static void InputField(GameObject go, string defaultText)
    {
        var inputField = go.AddComponent<TMP_InputField>();
        var textArea = Child(go, "TextArea");
        FillRT(textArea, new Vector2(10, 0), new Vector2(-10, 0));
        textArea.AddComponent<RectMask2D>();
        var tGO = Child(textArea, "Text");
        FillRT(tGO, Vector2.zero, Vector2.zero);
        var tTMP = tGO.AddComponent<TextMeshProUGUI>();
        tTMP.fontSize = 16; tTMP.color = Black;
        tTMP.alignment = TextAlignmentOptions.MidlineLeft;
        inputField.textComponent = tTMP;
        inputField.text = defaultText;
        inputField.textViewport = textArea.GetComponent<RectTransform>();
    }

    static void Btn(GameObject parent, string name, string label,
        Color bg, Color fg, bool border,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 pos, Vector2 size)
    {
        var go = Img(parent, name, bg);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        if (border) go.AddComponent<Outline>().effectColor = LineGray;
        go.AddComponent<Button>();
        var tGO = Child(go, "Text");
        FillRT(tGO, Vector2.zero, Vector2.zero);
        var t = tGO.AddComponent<TextMeshProUGUI>();
        t.text = label; t.fontSize = 18; t.color = fg;
        t.alignment = TextAlignmentOptions.Center; t.raycastTarget = false;
    }

    // ── PRIMITIVE HELPERS ─────────────────────────────────────────────────────

    // Creates a plain RectTransform child
    static GameObject Child(GameObject parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        return go;
    }

    // Creates a child with Image component
    static GameObject Img(GameObject parent, string name, Color color)
    {
        var go = Child(parent, name);
        go.AddComponent<Image>().color = color;
        return go;
    }

    // Stretch to fill parent using anchors (all offsets zero)
    static void Stretch(GameObject go, float minX, float minY, float maxX, float maxY)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(minX, minY);
        rt.anchorMax = new Vector2(maxX, maxY);
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    // Fill parent rect with given left/right inset offsets
    static void FillRT(GameObject go, Vector2 offsetMin, Vector2 offsetMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }
}
