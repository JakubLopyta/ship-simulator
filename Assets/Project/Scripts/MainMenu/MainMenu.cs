using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] ScenarioDynamicUI dynamicUI;
    [SerializeField] ScenarioListUI scenarioListUI;

    public void StartSimulation()
    {
        var values = dynamicUI?.CollectValues();
        if (values != null)
        {
            foreach (var kv in values)
                Debug.Log($"[MainMenu] {kv.Key} = {kv.Value}");

            // TODO: pass values to SimulationManager before loading scene
        }

        SceneManager.LoadScene("Simulation");
    }

    public void GoFreeMode()
    {
        SceneManager.LoadScene("Simulation");
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
