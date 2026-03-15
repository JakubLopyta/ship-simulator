using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeScaleUIController : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> multiplierLabelsWithParens;
    [SerializeField] private List<TextMeshProUGUI> multiplierLabelsPlain;

    public static event Action<float> OnTimeScaleChanged;

    public void ChangeTimeScale(float value)
    {
        int multiplier = (int)value;
        foreach (var label in multiplierLabelsWithParens)
            label.text = "(" + multiplier + "×)";
        foreach (var label in multiplierLabelsPlain)
            label.text = multiplier + "×";
        OnTimeScaleChanged?.Invoke(value);
    }
}
