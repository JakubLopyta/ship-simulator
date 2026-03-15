using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;

public class Weather : MonoBehaviour
{
    private double temperature = 21.0; // Temperatura w stopniach Celsjusza, domyślnie 21
    private double windSpeed; // Siła wiatru wyrażona w m/s
    private double windDirection; // Kierunek wiatru (0 stopni - północ)
    private int time = 480; // Czas wyrażony w minutach (0 - 0:00, 18:32 - 1112 (minuty od zera), domyślnie 8:00

    private bool isRaining; // true - pada, false - nie pada
    private bool isStorm; // true - burza, false - nie ma burzy
    private bool isFog; // true - mgła, false - nie ma mgły

    private double visibility; // Widoczność wyrażona w metrach
    private double waveHeight; // Wysokość fal wyrażona w metrach
    private double pressure; // Ciśnienie atmosferyczne wyrażone w hPa
    private double humidity; // Wilgotność powietrza wyrażona w %
    private float rainIntensity;

    [Header("Scene References")]
    public Light sunLight;

    [Header("Particles")]
    public GameObject rainParticles;
    public GameObject thunderstormParticles;
    public GameObject fogParticles;

    [Header("State Overrides")]
    public bool rainEnabled = false;
    public bool thunderstormEnabled = false;
    public bool fogEnabled = false;

    public bool SimulationRunning = true;

    private float timeAccumulator;
    private float desiredTimeScale = 1f;
    private float simulationSpeed = 0f; // 0 = zapauzowany, >0 = prędkość symulacji

    [SerializeField]
    [Range(0, 100)]
    private float _rainIntensity = 0.0f;

    [SerializeField]
    [Range(0, 100)]
    private float _fogDensity = 0.0f;

    [SerializeField]
    [Range(0, 1)]
    private float _thunderstormIntensity = 0.0f;

    private bool previousRainState;
    private bool previousThunderstormState;
    private bool previousFogState;

    // --- Properties ---

    public float RainIntensity
    {
        get => rainIntensity;
        set
        {
            _rainIntensity = Mathf.Round(value);
            var ps = rainParticles.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            emission.rateOverTime = _rainIntensity;
            rainIntensity = _rainIntensity;
        }
    }
    public float FogDensity
    {
        get => _fogDensity;
        set
        {
            _fogDensity = Mathf.Round(value);
            var ps = fogParticles.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            emission.rateOverTime = _fogDensity;
        }
    }
    public float ThunderstormIntensity
    {
        get => _thunderstormIntensity;
        set
        {
            _thunderstormIntensity = value;
            var ps = thunderstormParticles.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            emission.rateOverTime = _thunderstormIntensity;
        }
    }
    public double Temperature { get => temperature; set => temperature = value; }
    public double WindSpeed { get => windSpeed; set => windSpeed = value; }
    public double WindDirection { get => windDirection; set => windDirection = value; }
    public int Time
    {
        get => time;
        set
        {
            if (value > 1439) // 1440 to godzina 23:59
            {
                // Jeżeli wartość czasu jest równa jakiejś dziwnej wartości to usuwa jej nadmiar np. 2881 (dwie doby) to zostanie jedna minuta
                time = value % 1439 - 1;
            }
            else if (value < 0)
            {
                throw new ArgumentException("Invalid time format. Expected value => (0 - 1439)");
            }
            else
            {
                time = value;
            }
        }
    }
    public bool IsRaining { get => isRaining; set => isRaining = value; }
    public bool IsStorm { get => isStorm; set => isStorm = value; }
    public bool IsFog { get => isFog; set => isFog = value; }
    public double Visibility { get => visibility; set => visibility = value; }
    public double WaveHeight { get => waveHeight; set => waveHeight = value; }
    public double Pressure { get => pressure; set => pressure = value; }
    public double Humidity {
        get => humidity;
        set {
            if (value >= 0 && value <= 100)
            {
                humidity = value;
            }
            else
            {
                throw new ArgumentException("Invalid humidity format. Expected value => (0 - 100)");
            }
        }
    }

    // --- Events ---

    public static event Action<bool> OnRain;
    public static event Action<bool> OnThunderstorm;
    public static event Action<bool> OnFog;
    public static event Action<string> OnTimeUpdated;

    // --- Weather presets ---

    // Normalna pogoda, spokojne morze.
    public void SetClear()
    {
        Temperature = 22;
        Visibility = 10000;
        WindSpeed = 1d;
        WaveHeight = 0.1d;
        isRaining = false;
        isStorm = false;
        isFog = false;
    }

    public void SetRain()
    {
        Temperature = 18;
        Visibility = 5000;
        WindSpeed = 3d;
        WaveHeight = 1.0d;
        IsRaining = true;
        IsStorm = false;
    }

    public void SetThunderstorm()
    {
        Temperature = 16;
        Visibility = 2000;
        WindSpeed = 8d;
        WaveHeight = 2.5d;
        IsRaining = true;
        IsStorm = true;
    }

    public void SetFog()
    {
        Temperature = 14;
        Visibility = 500;
        WindSpeed = 0.5d;
        WaveHeight = 0.2d;
        IsRaining = false;
        IsStorm = false;
        IsFog = true;
    }

    void Update()
    {
        timeAccumulator += UnityEngine.Time.unscaledDeltaTime * simulationSpeed;
        int newTime = (int)timeAccumulator % 1440;
        if (newTime != time)
        {
            time = newTime;
            OnTimeUpdated?.Invoke(GetTimeAsString());
        }
    }

    public string GetTimeAsString()
    {
        int hours = Time / 60;
        int minutes = Time % 60;
        return $"{hours:D2}:{minutes:D2}";
    }

    // --- Unity lifecycle ---

    void Start()
    {
        WeatherUIController.OnSunChanged += SetClear;
        WeatherUIController.OnRainChanged += SetRain;
        WeatherUIController.OnThunderstormChanged += SetThunderstorm;
        WeatherUIController.OnFogChanged += SetFog;
        WeatherUIController.OnWindSpeedChanged += v => WindSpeed = v;
        WeatherUIController.OnWindDirectionChanged += v => WindDirection = v;
        WeatherUIController.OnWaveSizeChanged += v => WaveHeight = v;
        WeatherUIController.OnVisibilityRangeChanged += v => Visibility = v;
        WeatherUIController.OnRainIntensityChanged += v => _rainIntensity = v * 10;
        WeatherUIController.OnFogIntensityChanged += v => _fogDensity = (float)(v * 0.6);
        WeatherUIController.OnThunderstormIntensityChanged += v => _thunderstormIntensity = v / 100;
        TimeScaleUIController.OnTimeScaleChanged += v => { desiredTimeScale = v; if (simulationSpeed > 0) simulationSpeed = v; };
        ToolbarUIController.OnPlay    += state => simulationSpeed = state ? desiredTimeScale : 0;
        ToolbarUIController.OnPause   += state => simulationSpeed = state ? 0 : desiredTimeScale;
        ToolbarUIController.OnStop    += _ => simulationSpeed = 0;
        ToolbarUIController.OnRestart += _ => simulationSpeed = 0;
        WeatherUIController.OnTimeChanged += v => { Time += v; timeAccumulator = time; };

        timeAccumulator = time; // synchronizacja z początkowym czasem (domyślnie 480 = 8:00)
        simulationSpeed = 0; // startujemy zapauzowani — Play musi być wciśnięty

        previousRainState = IsRaining || rainEnabled;
        previousThunderstormState = IsStorm || thunderstormEnabled;
        previousFogState = IsFog || fogEnabled;
        OnRain?.Invoke(previousRainState);
        OnThunderstorm?.Invoke(previousThunderstormState);
        OnFog?.Invoke(previousFogState);

        StartCoroutine(UpdateEveryTick());
    }

    void OnDestroy()
    {
        WeatherUIController.OnSunChanged -= SetClear;
        WeatherUIController.OnRainChanged -= SetRain;
        WeatherUIController.OnThunderstormChanged -= SetThunderstorm;
        WeatherUIController.OnFogChanged -= SetFog;
    }

    // --- Simulation loop ---

    IEnumerator UpdateEveryTick()
    {
        // Wszystko co jest tutaj wrzucone będzie się działo co sekunde
        while (true)
        {
            RainIntensity = _rainIntensity;
            FogDensity = _fogDensity;
            ThunderstormIntensity = _thunderstormIntensity;
            UpdateLighting();
            bool isRainActive = IsRaining || rainEnabled;
            bool isThunderstormActive = IsStorm || thunderstormEnabled;
            bool isFogActive = IsFog || fogEnabled;

            rainParticles.SetActive(isRainActive);
            thunderstormParticles.SetActive(isThunderstormActive);
            fogParticles.SetActive(isFogActive);

            if (isRainActive != previousRainState)
            {
                previousRainState = isRainActive;
                OnRain?.Invoke(isRainActive);
            }
            if (isThunderstormActive != previousThunderstormState)
            {
                previousThunderstormState = isThunderstormActive;
                OnThunderstorm?.Invoke(isThunderstormActive);
            }
            if (isFogActive != previousFogState)
            {
                previousFogState = isFogActive;
                OnFog?.Invoke(isFogActive);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void UpdateLighting()
    {
        Color lightColor;

        // Kolor światła zależny od pory dnia
        if (Time < 360 || Time > 1080) // Noc
        {
            lightColor = new Color(0.1f, 0.1f, 0.2f); // zimny niebieski
        }
        else if (Time < 480) // Świt (6:00–8:00)
        {
            float d = Mathf.InverseLerp(360, 480, Time);
            lightColor = Color.Lerp(new Color(0.1f, 0.1f, 0.2f), new Color(1.0f, 0.7f, 0.4f), d);
        }
        else if (Time > 960 && Time <= 1080) // Zmierzch (16:00–18:00)
        {
            float d = Mathf.InverseLerp(960, 1080, Time);
            lightColor = Color.Lerp(new Color(1.0f, 0.7f, 0.4f), new Color(0.1f, 0.1f, 0.2f), d);
        }
        else // Dzień
        {
            lightColor = Color.white;
        }

        sunLight.color = lightColor;

        Material skyboxMat = RenderSettings.skybox;

        // Obrót słońca
        sunLight.transform.rotation = Quaternion.Euler(new Vector3((Time / 1440f) * 360f - 90f, 170f, 0f));

        // Jasność światła
        float intensity;
        if (Time < 240 || Time >= 1440) // Głęboka noc (0:00–4:00)
        {
            intensity = 0.05f;
        }
        else if (Time >= 240 && Time < 360) // Wschód (4:00–6:00)
        {
            float dawnFactor = Mathf.InverseLerp(240, 360, Time);
            intensity = Mathf.Lerp(0.05f, 1.0f, dawnFactor);
        }
        else if (Time >= 360 && Time < 1260) // Dzień (6:00–21:00)
        {
            intensity = 1.0f;
        }
        else // Zmrok (21:00–24:00)
        {
            float duskFactor = Mathf.InverseLerp(1260, 1440, Time);
            intensity = Mathf.Lerp(1.0f, 0.05f, duskFactor);
        }

        sunLight.intensity = intensity;
        RenderSettings.ambientIntensity = intensity;
        RenderSettings.reflectionIntensity = intensity;

        float exposure = Mathf.Lerp(0.2f, 1.0f, intensity);
        skyboxMat.SetFloat("_Exposure", exposure);
    }
}
