using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUIController : MonoBehaviour
{
    public OrbitCamera orbitCamera;

    public Ship shipReference;
    public Weather weatherReference;

    public Slider enginePowerSlider;
    public TMP_InputField enginePowerField;

    public Slider RudderSlider;
    public TMP_InputField RudderField;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI rotText;

    public TextMeshProUGUI cogText;
    public TextMeshProUGUI hdgText;
    public TextMeshProUGUI sogText;

    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;

    private bool isEditingEngine = false;
    private bool isEditingRudder = false;

    public static event Action<float> OnEnginePowerChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enginePowerSlider.onValueChanged.AddListener(OnEnginePowerSliderChanged);
        enginePowerField.onEndEdit.AddListener(OnEnginePowerFieldChanged);
        enginePowerField.onSelect.AddListener((_) => isEditingEngine = true);
        enginePowerField.onDeselect.AddListener((_) => isEditingEngine = false);

        RudderSlider.onValueChanged.AddListener(OnRudderSliderChanged);
        RudderField.onEndEdit.AddListener(OnRudderChanged);
        RudderField.onSelect.AddListener((_) => isEditingRudder = true);
        RudderField.onDeselect.AddListener((_) => isEditingRudder = false);
    }
    void OnDestroy()
    {
        enginePowerSlider.onValueChanged.RemoveListener(OnEnginePowerSliderChanged);
        enginePowerField.onEndEdit.RemoveListener(OnEnginePowerFieldChanged);
        enginePowerField.onSelect.RemoveAllListeners();
        enginePowerField.onDeselect.RemoveAllListeners();

        RudderSlider.onValueChanged.RemoveListener(OnRudderSliderChanged);
        RudderField.onEndEdit.RemoveListener(OnRudderChanged);
        RudderField.onSelect.RemoveAllListeners();
        RudderField.onDeselect.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (shipReference == null) return;

        speedText.text = shipReference.Speed.ToString() + " m/s";
        rotText.text = Math.Floor(shipReference.Rot).ToString() + "�/s";
        cogText.text = Math.Floor(shipReference.Cog).ToString() + "�";
        hdgText.text = Math.Floor(shipReference.Hdg).ToString() + "�";
        sogText.text = shipReference.Sog.ToString() + " m/s";
        latitudeText.text = Math.Round(shipReference.LatitudeDeg, 4).ToString();
        longitudeText.text = Math.Round(shipReference.LongitudeDeg, 4).ToString();

		if (!isEditingEngine)
        {
            enginePowerField.text = (shipReference.EnginePower * 100).ToString("F0");
            enginePowerSlider.value = (float)(shipReference.EnginePower * 100);
        }

        if (!isEditingRudder)
        {
            RudderField.text = shipReference.Rudder.ToString("F1");
            RudderSlider.value = (float)shipReference.Rudder;
        }
    }
    public void SetSelectedShip(Ship newShip)
    {
        shipReference = newShip;

        if (orbitCamera != null && newShip != null)
        {
            orbitCamera.SetTarget(newShip.transform);
        }
    }

    private void OnRudderSliderChanged(float value)
    {

        shipReference.Rudder = value;
    }

    private void OnRudderChanged(string text)
    {

        if (double.TryParse(text, out double value))
        {
            shipReference.Rudder = value;
        }
    }

    private void OnEnginePowerSliderChanged(float value)
    {
        float newValue = value / 100f;
        shipReference.EnginePower = newValue;
        OnEnginePowerChanged?.Invoke(newValue);
    }

    private void OnEnginePowerFieldChanged(string text)
    {

        if (float.TryParse(text, out float value))
        {
            float newValue = 0;
            if (value > 100) newValue = value / 100f;
            else if (value < 0) newValue = 0;
            else newValue = value / 100f;
            shipReference.EnginePower = newValue;
            OnEnginePowerChanged?.Invoke(newValue);
        }
    }
}
