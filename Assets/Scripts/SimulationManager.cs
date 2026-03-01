using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Range(0,20)]
    public float TimeMultiplier = 1;

    void Update()
    {
        Time.timeScale = Mathf.Max(0, TimeMultiplier);
    }
}
