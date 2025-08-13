using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class WeatherController : MonoBehaviour
{
    public Weather weather;
    
    public Light sunLight;
    public int timeMultiplier = 1;
    public bool rainEnabled = false;
    public bool thunderstormEnabled = false;
    public bool fogEnabled = false;

    [Header("Particles")]
    public GameObject rainParticles;
    public GameObject thunderstormParticles;
    public GameObject fogParticles;

    [Header("Labels")]
    public TextMeshProUGUI timeLabel;
    public TextMeshProUGUI wavesSizeLabel;
    public TextMeshProUGUI visibilityRangeLabel;
    public TextMeshProUGUI windLabel;

    [Header("Buttons")]
    public Button sunnyButton;
    public Button rainyButton;
    public Button foggyButton;
    public Button thunderstormButton;

    [Header("Sliders")]
    public Slider windSpeedSlider;
    public Slider windDirectionSlider;
    public Slider wavesSizeSlider;
    public Slider visibilityRangeSlider;

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
            var emissionRate = _rainIntensity * 100.0f;
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
    [Range(0, 60)]
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
            if (timeLabel == null)
            {
                Debug.LogWarning("Time label is not assigned in the WeatherController.");
            }
            else {
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


