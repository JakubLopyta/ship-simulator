using UnityEngine;
using UnityEngine.UIElements;

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
    private float fogDensity; // Gęstość mgły
    private float thunderstormIntensity; // Intensywność burzy

    public bool SimulationRunning = false;

    public float RainIntensity { get => rainIntensity; set => rainIntensity = value; }
    public float FogDensity { get => fogDensity; set => fogDensity = value; }
    public float ThunderstormIntensity { get => thunderstormIntensity; set => thunderstormIntensity = value; }
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
                throw new System.ArgumentException("Invalid time format. Expected value => (0 - 1439)");
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
                throw new System.ArgumentException("Invalid humidity format. Expected value => (0 - 100)");
            }
        }
    }

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
    public void SetWindy()
    {
        Temperature = 20;
        Visibility = 8000;
        WindSpeed = 6f;
        WaveHeight = 1.8f;
        IsRaining = false;
        IsStorm = false;
    }

    public void AdvanceTime(int minutes = 1)
    {
        if (SimulationRunning)
        {
            Time += minutes;
        }
    }

    public string GetTimeAsString()
    {
        int hours = Time / 60;
        int minutes = Time % 60;
        return $"{hours:D2}:{minutes:D2}";
    }

}
