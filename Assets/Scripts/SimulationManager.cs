using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public float TimeMultiplier = 1;

    void Update()
    {
        Time.timeScale = TimeMultiplier;
    }
}
