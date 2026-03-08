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

    [Header("Buttons")]
    public Button playButton;
    public Button stopButton;
    public Button pauseButton;
    public Button returnButton;

    private Color translucentButtonColor = new Color32(0, 0, 0, 0);
    private Color selectedButtonColor = new Color32(78, 101, 192, 190);

    private bool isEditingEngine = false;
    private bool isEditingRudder = false;

    public static event Action<float> OnEnginePowerChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button playBtn = playButton.GetComponent<Button>();
        playBtn.onClick.AddListener(OnPlayButtonClick);
        Button stopBtn = stopButton.GetComponent<Button>();
        stopBtn.onClick.AddListener(OnStopButtonClick);
        Button pauseBtn = pauseButton.GetComponent<Button>();
        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        Button returnBtn = returnButton.GetComponent<Button>();
        returnBtn.onClick.AddListener(OnReturnButtonClick);

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
        stopButton.onClick.RemoveListener(OnStopButtonClick);
        pauseButton.onClick.RemoveListener(OnStopButtonClick);
        returnButton.onClick.RemoveListener(OnStopButtonClick);

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
            enginePowerSlider.value = (shipReference.EnginePower * 100);
        }

        if (!isEditingRudder)
        {
            RudderField.text = shipReference.RudderTarget.ToString("F1");
            RudderSlider.value = shipReference.RudderTarget;
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

    private void OnReturnButtonClick()
    {
        shipReference.transform.position = Vector3.zero;
        shipReference.transform.rotation = Quaternion.identity;
    }
    private void OnPauseButtonClick()
    {
        shipReference.simulationRunning = false;
        weatherReference.SimulationRunning = false;
        PlayButtonPressed(shipReference.simulationRunning);
    }
    private void OnStopButtonClick()
    {
		shipReference.transform.position = Vector3.zero;
		shipReference.transform.rotation = Quaternion.identity;

		shipReference.simulationRunning = false;
        weatherReference.SimulationRunning = false;
        PlayButtonPressed(shipReference.simulationRunning);
    }
    private void OnPlayButtonClick()
    {
        shipReference.simulationRunning = true;
        weatherReference.SimulationRunning = true;
        PlayButtonPressed(shipReference.simulationRunning);
    }

    public void PlayButtonPressed(bool simulationState)
    {
        ColorBlock colorBlock = playButton.colors;
        colorBlock.normalColor = simulationState ? selectedButtonColor : translucentButtonColor;
        playButton.colors = colorBlock;
    }

    private void OnRudderSliderChanged(float value)
    {

        shipReference.RudderTarget = value;
    }

    private void OnRudderChanged(string text)
    {

        if (float.TryParse(text, out float value))
        {
            shipReference.RudderTarget = value;
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
