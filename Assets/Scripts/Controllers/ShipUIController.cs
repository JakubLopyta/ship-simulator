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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button playBtn = playButton.GetComponent<Button>();
        playBtn.onClick.AddListener(onPlayBtnClick);
        Button stopBtn = stopButton.GetComponent<Button>();
        stopBtn.onClick.AddListener(onStopBtnClick);
        Button pauseBtn = pauseButton.GetComponent<Button>();
        pauseBtn.onClick.AddListener(onPauseBtnClick);
        Button returnBtn = returnButton.GetComponent<Button>();
        returnBtn.onClick.AddListener(onReturnBtnClick);

        enginePowerSlider.onValueChanged.AddListener(OnEnginePowerSliderChanged);
        enginePowerField.onEndEdit.AddListener(OnEnginePowerFieldChanged);
        enginePowerField.onSelect.AddListener((_) => isEditingEngine = true);
        enginePowerField.onDeselect.AddListener((_) => isEditingEngine = false);

        RudderSlider.onValueChanged.AddListener(OnRudderSliderChanged);
        RudderField.onEndEdit.AddListener(OnRudderChanged);
        RudderField.onSelect.AddListener((_) => isEditingRudder = true);
        RudderField.onDeselect.AddListener((_) => isEditingRudder = false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shipReference == null) return;

        speedText.text = shipReference.Speed.ToString() + " m/s";
        rotText.text = Math.Floor(shipReference.Rot).ToString() + "°/s";
        cogText.text = Math.Floor(shipReference.Cog).ToString() + "°";
        hdgText.text = Math.Floor(shipReference.Hdg).ToString() + "°";
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

    private void onReturnBtnClick()
    {
        shipReference.transform.position = Vector3.zero;
        shipReference.transform.rotation = Quaternion.identity;
    }
    private void onPauseBtnClick()
    {
        shipReference.simulationRunning = false;
        weatherReference.SimulationRunning = false;
        PlayButtonPressed(shipReference.simulationRunning);
    }
    private void onStopBtnClick()
    {
		shipReference.transform.position = Vector3.zero;
		shipReference.transform.rotation = Quaternion.identity;

		shipReference.simulationRunning = false;
        weatherReference.SimulationRunning = false;
        PlayButtonPressed(shipReference.simulationRunning);
    }
    private void onPlayBtnClick()
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

        shipReference.EnginePower = value / 100f;
    }

    private void OnEnginePowerFieldChanged(string text)
    {

        if (float.TryParse(text, out float value))
        {
            if (value > 100) shipReference.EnginePower = value / 100;
            else if (value < 0) shipReference.EnginePower = 0;
            else shipReference.EnginePower = value / 100;
        }
    }
}
