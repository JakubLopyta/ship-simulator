using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeatherController : MonoBehaviour
{
    public Weather weather;
    
    public Light sunLight;
    public bool rainEnabled = false;
    public bool thunderstormEnabled = false;
    public bool fogEnabled = false;
    [Header("Time Management")]
    [SerializeField]
    public int timeMultiplier = 1;

    [Header("Particles")]
    public GameObject rainParticles;
    public GameObject thunderstormParticles;
    public GameObject fogParticles;

    [Header("Labels")]
    public List<TextMeshProUGUI> timeLabels;
    public List<TextMeshProUGUI> timeMultiplierLabels;
    public TextMeshProUGUI wavesSizeLabel;
    public TextMeshProUGUI visibilityRangeLabel;
    public TextMeshProUGUI windLabel;
    public TextMeshProUGUI thunderstormIntensityLabel;
    public TextMeshProUGUI rainIntensityLabel;
    public TextMeshProUGUI fogIntensityLabel;

    [Header("Buttons")]
    public Button addMinuteButton;
    public Button removeMinuteButton;
    public Button addHourButton;
    public Button removeHourButton;
    public Button addSecondButton;
    public Button removeSecondButton;
    public Button sunnyButton;
    public Button rainyButton;
    public Button foggyButton;
    public Button thunderstormButton;

    [Header("Sliders")]
    public Slider windSpeedSlider;
    public Slider windDirectionSlider;
    public Slider wavesSizeSlider;
    public Slider visibilityRangeSlider;
    public Slider rainIntensitySlider;
    public Slider thunderstormIntensitySlider;
    public Slider fogIntensitySlider;
    public Slider timeScaleSlider;

    private Color translucentButtonColor = new Color32(0, 0, 0, 0);
    private Color selectedButtonColor = new Color32(78, 101, 192, 190);

    public float rainIntensity
    {
        get
        {
            return weather.RainIntensity;
        }
        set
        {
            _rainIntensity = Mathf.Round(value);
            var ps = rainParticles.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            var emissionRate = _rainIntensity;
            emission.rateOverTime = emissionRate;
            weather.RainIntensity = _rainIntensity;
        }
    }
    public float fogDensity
    {
        get
        {
            return _fogDensity;
        }
        set
        {
            _fogDensity = Mathf.Round(value);
            var ps = fogParticles.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            var emissionRate = _fogDensity;
            emission.rateOverTime = emissionRate;
            weather.FogDensity = _fogDensity;
        }
    }
    public float thunderstormIntensity
    {
        get
        {
            return _thunderstormIntensity;
        }
        set
        {
            var ps = thunderstormParticles.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            var emissionRate = _thunderstormIntensity;
            emission.rateOverTime = emissionRate;
            weather.ThunderstormIntensity = _thunderstormIntensity;
        }
    }
    [SerializeField]
    [Range(0, 100)]
    private float _rainIntensity = 0.0f;

    [SerializeField]
    [Range(0, 100)]
    private float _fogDensity = 0.0f;

    [SerializeField]
    [Range(0, 1)]
    private float _thunderstormIntensity = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
       Button sunnyBtn = sunnyButton.GetComponent<Button>();
       sunnyBtn.onClick.AddListener(() =>
       {
           weather.SetClear();
       });
         Button rainyBtn = rainyButton.GetComponent<Button>();
         rainyBtn.onClick.AddListener(() =>
            {
                weather.SetRain();
            });
            Button foggyBtn = foggyButton.GetComponent<Button>();
            foggyBtn.onClick.AddListener(() =>
            {
                weather.SetFog();
            });
            Button thunderstormBtn = thunderstormButton.GetComponent<Button>();
            thunderstormBtn.onClick.AddListener(() =>
            {
                weather.SetThunderstorm();
            });
        Button addHourBtn = addHourButton.GetComponent<Button>();
        addHourBtn.onClick.AddListener(() =>
        {
            weather.Time += 60;
        });
        Button removeHourBtn = removeHourButton.GetComponent<Button>();
        removeHourBtn.onClick.AddListener(() =>
        {
            weather.Time -= 60;
        });
        Button addSecondBtn = addSecondButton.GetComponent<Button>();
        addSecondBtn.onClick.AddListener(() =>
        {
            weather.Time += 1 / 60;
        });
        Button removeSecondBtn = removeSecondButton.GetComponent<Button>();
        removeSecondBtn.onClick.AddListener(() =>
        {
            weather.Time -= 1 / 60;
        });
        Button addMinuteBtn = addMinuteButton.GetComponent<Button>();
        addMinuteBtn.onClick.AddListener(() => {
            weather.Time += 1;
        });
        Button removeMinuteBtn = removeMinuteButton.GetComponent<Button>();
        removeMinuteBtn.onClick.AddListener(() => {
            weather.Time -= 1;
        });

        windDirectionSlider.onValueChanged.AddListener((float value) => {
            weather.WindDirection = value;
            windLabel.text = weather.WindSpeed + "kts @" + weather.WindDirection + "°";
        }
        );
        windSpeedSlider.onValueChanged.AddListener((float value) => {
            weather.WindSpeed = value;
            windLabel.text = weather.WindSpeed + "kts @" + weather.WindDirection + "°";
        });
        wavesSizeSlider.onValueChanged.AddListener((float value) => {
            weather.WaveHeight = value;
            wavesSizeLabel.text = value.ToString() + " m";
        });
        visibilityRangeSlider.onValueChanged.AddListener((float value) => {
            visibilityRangeLabel.text = value.ToString() + " m";
            weather.Visibility = value;
          });
        rainIntensitySlider.onValueChanged.AddListener((float value) =>
        {
            rainIntensityLabel.text = value.ToString() + "%";
            rainIntensity = value * 10;
        });
        fogIntensitySlider.onValueChanged.AddListener((float value) =>
        {
            fogIntensityLabel.text = value.ToString() + "%";
            fogDensity = (float)(value * (double)0.6);
        });
        thunderstormIntensitySlider.onValueChanged.AddListener((float value) =>
        {
            thunderstormIntensityLabel.text = value.ToString() + "%";
            thunderstormIntensity = value / 100;
        });
        timeScaleSlider.onValueChanged.AddListener((float value) =>
        {
            timeMultiplier = (int)value;
            foreach (var timeMultiplierLabel in timeMultiplierLabels)
            {
                timeMultiplierLabel.text = "(" + timeMultiplier.ToString() + "×)";
            }
        });

        StartCoroutine(UpdateEverySecond());
    }

    IEnumerator UpdateEverySecond()
    {
        // Wszystko co jest tutaj wrzucone będzie się działo co sekunde
        while (true)
        {
            rainIntensity = _rainIntensity;
            fogDensity = _fogDensity;
            thunderstormIntensity = _thunderstormIntensity;
            updateLighting();
            weather.AdvanceTime(timeMultiplier);
                foreach (var timeLabel in timeLabels)
                {
                    timeLabel.text = weather.GetTimeAsString();
                }
                
            if (weather.IsRaining || rainEnabled)
            {
                rainParticles.SetActive(true);
            }
            else
            {
                rainParticles.SetActive(false);
            }
            if (weather.IsStorm || thunderstormEnabled)
            {
                thunderstormParticles.SetActive(true);
            }
            else
            {
                thunderstormParticles.SetActive(false);
            }
            if (weather.IsFog || fogEnabled)
            {
                fogParticles.SetActive(true);
            }
            else
            {
                fogParticles.SetActive(false);
            }
            yield return new WaitForSeconds(1);
        }
    }

    void updateLighting()
    {
        Color lightColor;

        // Kolor światła zależny od pory dnia
        if (weather.Time < 360 || weather.Time > 1080) // Noc
        {
            lightColor = new Color(0.1f, 0.1f, 0.2f); // zimny niebieski
        }
        else if (weather.Time < 480) // Świt (6:00–8:00)
        {
            float d = Mathf.InverseLerp(360, 480, weather.Time);
            lightColor = Color.Lerp(new Color(0.1f, 0.1f, 0.2f), new Color(1.0f, 0.7f, 0.4f), d);
        }
        else if (weather.Time > 960 && weather.Time <= 1080) // Zmierzch (16:00–18:00)
        {
            float d = Mathf.InverseLerp(960, 1080, weather.Time);
            lightColor = Color.Lerp(new Color(1.0f, 0.7f, 0.4f), new Color(0.1f, 0.1f, 0.2f), d);
        }
        else // Dzień
        {
            lightColor = Color.white;
        }

        sunLight.color = lightColor;

        Material skyboxMat = RenderSettings.skybox;

        // Obrót słońca
        sunLight.transform.rotation = Quaternion.Euler(new Vector3((weather.Time / 1440f) * 360f - 90f, 170f, 0f));

        // Jasność światła
        float intensity;
        if (weather.Time < 240 || weather.Time >= 1440) // Głęboka noc (0:00–4:00)
        {
            intensity = 0.05f;
        }
        else if (weather.Time >= 240 && weather.Time < 360) // Wschód (4:00–6:00)
        {
            float dawnFactor = Mathf.InverseLerp(240, 360, weather.Time);
            intensity = Mathf.Lerp(0.05f, 1.0f, dawnFactor);
        }
        else if (weather.Time >= 360 && weather.Time < 1260) // Dzień (6:00–21:00)
        {
            intensity = 1.0f;
        }
        else // Zmrok (21:00–24:00)
        {
            float duskFactor = Mathf.InverseLerp(1260, 1440, weather.Time);
            intensity = Mathf.Lerp(1.0f, 0.05f, duskFactor);
        }

        sunLight.intensity = intensity;
        RenderSettings.ambientIntensity = intensity;
        RenderSettings.reflectionIntensity = intensity;

        float exposure = Mathf.Lerp(0.2f, 1.0f, intensity);
        skyboxMat.SetFloat("_Exposure", exposure);
    }


    public void SetToFog() // Changed from local function to a public method
    {
        weather.SetFog();
        RenderSettings.fogDensity = 0.005f;
    }
}


