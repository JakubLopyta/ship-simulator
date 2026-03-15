using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUIController : MonoBehaviour
{
    [Header("Engine UI")]
    [SerializeField] private Slider enginePowerSlider;
    [SerializeField] private TMP_InputField enginePowerField;

    [Header("Rudder UI")]
    [SerializeField] private Slider rudderSlider;
    [SerializeField] private TMP_InputField rudderField;

    [Header("Info Texts")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rotText;
    [SerializeField] private TextMeshProUGUI cogText;
    [SerializeField] private TextMeshProUGUI hdgText;
    [SerializeField] private TextMeshProUGUI sogText;
    [SerializeField] private TextMeshProUGUI latitudeText;
    [SerializeField] private TextMeshProUGUI longitudeText;

    private Ship ship;
    private bool isEditingEngine = false;
    private bool isEditingRudder = false;

    public static event Action<float> OnEnginePowerChanged;

    private void Start()
    {
        GameObject shipObj = GameObject.FindGameObjectWithTag("Ship");
        if (shipObj != null)
            ship = shipObj.GetComponent<Ship>();
        else
            Debug.LogWarning("ShipUIController: No GameObject with tag 'Ship' found.");

        enginePowerSlider.onValueChanged.AddListener(OnEnginePowerSliderChanged);
        enginePowerField.onEndEdit.AddListener(OnEnginePowerFieldChanged);
        enginePowerField.onSelect.AddListener((_) => isEditingEngine = true);
        enginePowerField.onDeselect.AddListener((_) => isEditingEngine = false);

        rudderSlider.onValueChanged.AddListener(OnRudderSliderChanged);
        rudderField.onEndEdit.AddListener(OnRudderFieldChanged);
        rudderField.onSelect.AddListener((_) => isEditingRudder = true);
        rudderField.onDeselect.AddListener((_) => isEditingRudder = false);
    }

    private void OnDestroy()
    {
        enginePowerSlider.onValueChanged.RemoveListener(OnEnginePowerSliderChanged);
        enginePowerField.onEndEdit.RemoveListener(OnEnginePowerFieldChanged);
        enginePowerField.onSelect.RemoveAllListeners();
        enginePowerField.onDeselect.RemoveAllListeners();

        rudderSlider.onValueChanged.RemoveListener(OnRudderSliderChanged);
        rudderField.onEndEdit.RemoveListener(OnRudderFieldChanged);
        rudderField.onSelect.RemoveAllListeners();
        rudderField.onDeselect.RemoveAllListeners();
    }

    private void Update()
    {
        if (ship == null) return;

        speedText.text = Math.Round(ship.Speed, 2).ToString("F2") + " m/s";
        rotText.text = Math.Round(ship.Rot, 2).ToString("F2") + "°/s";
        cogText.text = Math.Round(ship.Cog, 2).ToString("F2") + "°";
        hdgText.text = Math.Round(ship.Hdg, 2).ToString("F2") + "°";
        sogText.text = Math.Round(ship.Sog, 2).ToString("F2") + " m/s";
        latitudeText.text = Math.Round(ship.LatitudeDeg, 4).ToString("F4");
        longitudeText.text = Math.Round(ship.LongitudeDeg, 4).ToString("F4");

        if (!isEditingEngine)
        {
            float engineDisplay = Mathf.Round(ship.EnginePower * 100);
            enginePowerField.text = engineDisplay.ToString("F0");
            SetSliderWithoutNotify(enginePowerSlider, engineDisplay);
        }

        if (!isEditingRudder)
        {
            float rudderDisplay = (float)Math.Round(ship.Rudder, 2);
            rudderField.text = rudderDisplay.ToString("F2");
            SetSliderWithoutNotify(rudderSlider, rudderDisplay);
        }
    }

    private void SetSliderWithoutNotify(Slider slider, float value)
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.value = value;

        if (slider == enginePowerSlider)
            slider.onValueChanged.AddListener(OnEnginePowerSliderChanged);
        else if (slider == rudderSlider)
            slider.onValueChanged.AddListener(OnRudderSliderChanged);
    }

    private void OnRudderSliderChanged(float value)
    {
        if (ship == null) return;
        float snapped = Mathf.Round(value * 10f) / 10f;
        ship.Rudder = snapped;
        SetSliderWithoutNotify(rudderSlider, snapped);
    }

    private void OnRudderFieldChanged(string text)
    {
        if (ship == null) return;
        if (double.TryParse(text, out double value))
            ship.Rudder = value;
    }

    private void OnEnginePowerSliderChanged(float value)
    {
        if (ship == null) return;
        float newValue = value / 100f;
        ship.EnginePower = newValue;
        OnEnginePowerChanged?.Invoke(newValue);
    }

    private void OnEnginePowerFieldChanged(string text)
    {
        if (ship == null) return;
        if (float.TryParse(text, out float value))
        {
            float newValue = Mathf.Clamp(value, 0f, 100f) / 100f;
            ship.EnginePower = newValue;
            OnEnginePowerChanged?.Invoke(newValue);
        }
    }
}
