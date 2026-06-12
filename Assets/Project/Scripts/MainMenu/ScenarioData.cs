using System;

[Serializable]
public class ScenarioDefinition
{
    public string scenarioId;
    public string title;
    public ScenarioParameter[] parameters;
}

[Serializable]
public class ScenarioParameter
{
    public string type;              // "header" | "dropdown" | "slider" | "input" | "row"
    public string id;                // klucz do CollectValues()
    public string label;
    public string[] options;         // dropdown
    public float min;                // slider
    public float max;                // slider
    public float defaultValue;       // slider
    public string defaultText;       // input
    public ScenarioRowChild[] children; // row — separate type to avoid JsonUtility recursive-type limitation
}

/// <summary>
/// Leaf parameter inside a "row" container. Identical fields to ScenarioParameter
/// but without 'children', so JsonUtility can serialize it without hitting the
/// recursive-type restriction.
/// </summary>
[Serializable]
public class ScenarioRowChild
{
    public string type;         // "slider" | "dropdown" | "input"
    public string id;
    public string label;
    public string[] options;    // dropdown
    public float min;           // slider
    public float max;           // slider
    public float defaultValue;  // slider
    public string defaultText;  // input
}
