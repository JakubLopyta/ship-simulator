using UnityEngine;
// TODO: Zmiana szybkości symulacji
public class SimulationManager : MonoBehaviour
{
    [Range(0,50)]
    public float TimeMultiplier = 1;

    void Update()
    {
        Time.timeScale = Mathf.Max(0, TimeMultiplier);
    }
}
