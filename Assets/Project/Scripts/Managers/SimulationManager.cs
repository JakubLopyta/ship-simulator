using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] [Range(0, 50)] private float timeMultiplier = 1;

    void OnEnable()
    {
        TimeScaleUIController.OnTimeScaleChanged += ApplyTimeScale;
    }

    void OnDisable()
    {
        TimeScaleUIController.OnTimeScaleChanged -= ApplyTimeScale;
    }

    void ApplyTimeScale(float value)
    {
        timeMultiplier = value;
        Time.timeScale = Mathf.Max(0, timeMultiplier);
    }
}
