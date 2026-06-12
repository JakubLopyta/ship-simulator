using UnityEngine;

/// <summary>
/// ScriptableObject that wraps a scenario JSON file.
/// Drag a JSON TextAsset here, then click "Parse JSON" in the Inspector.
/// </summary>
[CreateAssetMenu(menuName = "Ship Simulator/Scenario Config", fileName = "NewScenarioConfig")]
public class ScenarioConfigAsset : ScriptableObject
{
    public TextAsset jsonFile;

    // Cached after parsing — not shown by default, use custom editor
    [HideInInspector] public ScenarioDefinition parsed;

    public bool TryParse(out string error)
    {
        error = null;
        if (jsonFile == null) { error = "No JSON file assigned."; return false; }
        try
        {
            parsed = JsonUtility.FromJson<ScenarioDefinition>(jsonFile.text);
            if (string.IsNullOrEmpty(parsed?.scenarioId))
            {
                error = "Parsed definition has no scenarioId.";
                return false;
            }
            return true;
        }
        catch (System.Exception e)
        {
            error = e.Message;
            return false;
        }
    }
}
