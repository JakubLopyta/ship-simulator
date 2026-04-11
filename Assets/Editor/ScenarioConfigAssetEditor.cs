using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScenarioConfigAsset))]
public class ScenarioConfigAssetEditor : Editor
{
    static readonly Color HeaderColor  = new Color(0.18f, 0.50f, 0.93f, 0.15f);
    static readonly Color SliderColor  = new Color(0.20f, 0.75f, 0.30f, 0.15f);
    static readonly Color DropdownColor= new Color(0.93f, 0.60f, 0.18f, 0.15f);
    static readonly Color InputColor   = new Color(0.60f, 0.18f, 0.93f, 0.15f);

    string parseError;
    bool parsed;

    public override void OnInspectorGUI()
    {
        var asset = (ScenarioConfigAsset)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("jsonFile"));
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
        {
            parsed = false;
            parseError = null;
        }

        EditorGUILayout.Space(4);

        if (GUILayout.Button("Parse JSON", GUILayout.Height(30)))
        {
            parsed = asset.TryParse(out parseError);
            if (parsed) EditorUtility.SetDirty(asset);
        }

        if (!string.IsNullOrEmpty(parseError))
        {
            EditorGUILayout.HelpBox(parseError, MessageType.Error);
            return;
        }

        if (!parsed && asset.parsed == null) return;

        var def = asset.parsed;
        if (def == null) return;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField($"ID: {def.scenarioId}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Title: {def.title}");

        if (def.parameters == null || def.parameters.Length == 0)
        {
            EditorGUILayout.HelpBox("No parameters defined.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

        foreach (var p in def.parameters)
        {
            Color bg = p.type switch
            {
                "header"   => HeaderColor,
                "slider"   => SliderColor,
                "dropdown" => DropdownColor,
                "input"    => InputColor,
                _          => Color.clear
            };

            DrawParamRow(p, bg);
        }
    }

    void DrawParamRow(ScenarioParameter p, Color bg)
    {
        var rect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(rect, bg);

        EditorGUILayout.BeginHorizontal();

        // Type badge
        var badgeStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontStyle = FontStyle.Bold,
            fixedWidth = 70
        };
        EditorGUILayout.LabelField($"[{p.type?.ToUpper()}]", badgeStyle);

        // Label
        EditorGUILayout.LabelField(p.label ?? "(no label)", GUILayout.ExpandWidth(true));

        // ID
        if (!string.IsNullOrEmpty(p.id))
            EditorGUILayout.LabelField($"id: {p.id}", EditorStyles.miniLabel, GUILayout.Width(140));

        EditorGUILayout.EndHorizontal();

        // Extra info per type
        switch (p.type)
        {
            case "slider":
                EditorGUILayout.LabelField($"    range: {p.min} – {p.max}   default: {p.defaultValue}", EditorStyles.miniLabel);
                break;
            case "dropdown":
                if (p.options != null && p.options.Length > 0)
                    EditorGUILayout.LabelField($"    options: {string.Join(", ", p.options)}", EditorStyles.miniLabel);
                break;
            case "input":
                if (!string.IsNullOrEmpty(p.defaultText))
                    EditorGUILayout.LabelField($"    default: \"{p.defaultText}\"", EditorStyles.miniLabel);
                break;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }
}
