using UnityEngine;
using System.Collections;
using TMPro;

public class WeatherController : MonoBehaviour
{
    public Weather weather;
    public TextMeshProUGUI timeLabel;
    public Light sunLight;
    public int timeMultiplier = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpdateEverySecond());
    }

    IEnumerator UpdateEverySecond()
    {
        // Wszystko co jest tutaj wrzucone będzie się działo co sekunde
        while (true)
        {
            updateLighting();
            weather.AdvanceTime(timeMultiplier);
            timeLabel.text = weather.GetTimeAsString();
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


