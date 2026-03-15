using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum WeatherButton
{
    SUN_BUTTON = 0, 
    FOG_BUTTON = 1, 
    RAIN_BUTTON = 2, 
    THUNDERSTORM_BUTTON = 3
}

public enum WeatherSlider
{
    WIND_SPEED, WIND_DIRECTION, WAVE_SIZE, VISIBILITY_RANGE, RAIN_INTENSITY, THUNDERSTORM_INTENSITY, FOG_INTENSITY
}

public class WeatherUIController : MonoBehaviour
{
    [Header("Weather Buttons")]
    [SerializeField] private Button sunButton;
    [SerializeField] private Button fogButton;
    [SerializeField] private Button rainButton;
    [SerializeField] private Button thunderstormButton;

    [Header("Weather Control")]
    [SerializeField] private List<TextMeshProUGUI> timeLabelList;
    [SerializeField] private TextMeshProUGUI wavesSizeLabel;
    [SerializeField] private TextMeshProUGUI visibilityRangeLabel;
    [SerializeField] private TextMeshProUGUI windLabel;
    [SerializeField] private TextMeshProUGUI thunderstormIntensityLabel;
    [SerializeField] private TextMeshProUGUI rainIntensityLabel;
    [SerializeField] private TextMeshProUGUI fogIntensityLabel;

    [Header("Sliders")]
    [SerializeField] private Slider windSpeedSlider;
    [SerializeField] private Slider windDirectionSlider;
    [SerializeField] private Slider waveSizeSlider;
    [SerializeField] private Slider visibilityRangeSlider;
    [SerializeField] private Slider rainIntensitySlider;
    [SerializeField] private Slider thunderstormIntensitySlider;
    [SerializeField] private Slider fogIntensitySlider;

    public static event Action OnSunChanged;
    public static event Action OnRainChanged;
    public static event Action OnThunderstormChanged;
    public static event Action OnFogChanged;
    public static event Action<float> OnRainIntensityChanged;
    public static event Action<float> OnFogIntensityChanged;
    public static event Action<float> OnThunderstormIntensityChanged;
    public static event Action<float> OnWindSpeedChanged;
    public static event Action<float> OnWindDirectionChanged;
    public static event Action<float> OnWaveSizeChanged;
    public static event Action<float> OnVisibilityRangeChanged;
    public static event Action<int> OnTimeChanged;

    private WeatherButton activeWeather = WeatherButton.SUN_BUTTON;
    private Color buttonNormalColor;

    void Start()
    {
        Weather.OnTimeUpdated += UpdateTimeLabels;
        if (sunButton != null)
            buttonNormalColor = sunButton.colors.normalColor;
        RefreshAllLabels();
        SelectButton(WeatherButton.SUN_BUTTON);
        OnSunChanged?.Invoke();
    }

    private void RefreshAllLabels()
    {
        if (windSpeedSlider)      ChangeWindSpeed(windSpeedSlider.value);
        if (windDirectionSlider)  ChangeWindDirection(windDirectionSlider.value);
        if (waveSizeSlider)       ChangeWaveSize(waveSizeSlider.value);
        if (visibilityRangeSlider) ChangeVisibilityRange(visibilityRangeSlider.value);
        if (rainIntensitySlider)  ChangeRainIntensity(rainIntensitySlider.value);
        if (thunderstormIntensitySlider) ChangeThunderstormIntensity(thunderstormIntensitySlider.value);
        if (fogIntensitySlider)   ChangeFogIntensity(fogIntensitySlider.value);
    }

    void OnDestroy()
    {
        Weather.OnTimeUpdated -= UpdateTimeLabels;
    }

    private void UpdateTimeLabels(string time)
    {
        foreach (var label in timeLabelList)
            label.text = time;
    }

    public void ChangeWeatherState(int button)
    {
        WeatherButton pressed = (WeatherButton)button;

        if (pressed != WeatherButton.SUN_BUTTON && pressed == activeWeather)
        {
            SelectButton(WeatherButton.SUN_BUTTON);
            OnSunChanged?.Invoke();
            return;
        }

        SelectButton(pressed);
        switch (pressed)
        {
            case WeatherButton.SUN_BUTTON:          OnSunChanged?.Invoke();          break;
            case WeatherButton.FOG_BUTTON:          OnFogChanged?.Invoke();          break;
            case WeatherButton.RAIN_BUTTON:         OnRainChanged?.Invoke();         break;
            case WeatherButton.THUNDERSTORM_BUTTON: OnThunderstormChanged?.Invoke(); break;
        }
    }

    private void SelectButton(WeatherButton weather)
    {
        activeWeather = weather;
        SetButtonActive(sunButton,          weather == WeatherButton.SUN_BUTTON);
        SetButtonActive(fogButton,          weather == WeatherButton.FOG_BUTTON);
        SetButtonActive(rainButton,         weather == WeatherButton.RAIN_BUTTON);
        SetButtonActive(thunderstormButton, weather == WeatherButton.THUNDERSTORM_BUTTON);
    }

    private void SetButtonActive(Button btn, bool active)
    {
        if (btn == null) return;
        ColorBlock cb = btn.colors;
        cb.normalColor   = active ? cb.pressedColor : buttonNormalColor;
        cb.selectedColor = cb.normalColor;
        btn.colors = cb;
    }

    private float windSpeed = 0f;
    private float windDirection = 0f;

    public void ChangeWindSpeed(float value)
    {
        windSpeed = value;
        windLabel.text = windSpeed + "kts @" + windDirection + "°";
        OnWindSpeedChanged?.Invoke(value);
    }

    public void ChangeWindDirection(float value)
    {
        windDirection = value;
        windLabel.text = windSpeed + "kts @" + windDirection + "°";
        OnWindDirectionChanged?.Invoke(value);
    }

    public void ChangeWaveSize(float value)
    {
        wavesSizeLabel.text = value + " m";
        OnWaveSizeChanged?.Invoke(value);
    }

    public void ChangeVisibilityRange(float value)
    {
        visibilityRangeLabel.text = value + " m";
        OnVisibilityRangeChanged?.Invoke(value);
    }

    public void ChangeRainIntensity(float value)
    {
        rainIntensityLabel.text = value + "%";
        OnRainIntensityChanged?.Invoke(value);
    }

    public void ChangeThunderstormIntensity(float value)
    {
        thunderstormIntensityLabel.text = value + "%";
        OnThunderstormIntensityChanged?.Invoke(value);
    }

    public void ChangeFogIntensity(float value)
    {
        fogIntensityLabel.text = value + "%";
        OnFogIntensityChanged?.Invoke(value);
    }

    public void ChangeTime(int minutes)
    {
        OnTimeChanged?.Invoke(minutes);
    }
}
