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
        Material skyboxMat = RenderSettings.skybox;

        sunLight.transform.rotation = Quaternion.Euler(new Vector3((weather.Time / 1440f) * 360f - 90f, 170f, 0f)); // Obraca fizyczne słońce
        double t = weather.Time / 1440d;
        float intensity;
        if (weather.Time < 360 || weather.Time > 1080)
        {
            intensity = 0.05f;
        }
        else
        {
            float normalizedDay = Mathf.InverseLerp(360, 1080, weather.Time);
            intensity = Mathf.Lerp(0.1f, 1.0f, Mathf.Sin(normalizedDay * Mathf.PI));
        }
        sunLight.intensity = intensity;
        RenderSettings.ambientIntensity = intensity;
        RenderSettings.reflectionIntensity = intensity;
        // Ekspozycja od ciemnej (noc) do jasnej (dzień)
        float exposure = Mathf.Lerp(0.2f, 1.0f, sunLight.intensity);
        skyboxMat.SetFloat("_Exposure", exposure);
    }

    public void SetToFog() // Changed from local function to a public method
    {
        weather.SetFog();
        RenderSettings.fogDensity = 0.005f;
    }
}


