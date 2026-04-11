using System;
using System.Linq;
using UnityEngine;

public static class ScenarioLoader
{
    public static ScenarioDefinition[] LoadAll()
    {
        TextAsset[] assets = Resources.LoadAll<TextAsset>("Scenarios");

        return assets
            .OrderBy(a => a.name)
            .Select(asset =>
            {
                try
                {
                    return JsonUtility.FromJson<ScenarioDefinition>(asset.text);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ScenarioLoader] Failed to parse '{asset.name}': {e.Message}");
                    return null;
                }
            })
            .Where(def => def != null && !string.IsNullOrEmpty(def.scenarioId))
            .ToArray();
    }

    public static ScenarioDefinition Load(string scenarioId)
    {
        TextAsset asset = Resources.Load<TextAsset>($"Scenarios/{scenarioId}");
        if (asset == null)
        {
            Debug.LogError($"[ScenarioLoader] Scenario file not found: Scenarios/{scenarioId}");
            return null;
        }
        return JsonUtility.FromJson<ScenarioDefinition>(asset.text);
    }
}
