using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip seaIdleSFX;
    [SerializeField] private AudioClip engineSFX;
    [SerializeField] private AudioClip rainSFX;
    [SerializeField] private AudioClip thunderstormSFX;
    [SerializeField] private AudioClip collisionSFX;
    [SerializeField] private List<AudioSource> sources = new List<AudioSource>();
    private AudioSource globalSource;
    private AudioSource seaSource;
    private AudioSource engineSource;
    private AudioSource rainSource;
    private AudioSource thunderstormSource;
    [SerializeField] private GameObject audioSourcePrefab;
    [Header("Volume")]
    [SerializeField] [Range(0.0f, 1f)] private float seaIdleVolume = 0.5f;
    [SerializeField] [Range(0.0f, 1f)] private float rainVolume = 0.5f;
    [SerializeField] [Range(0.0f, 1f)] private float thunderstormVolume = 0.5f;
    void Start()
    {
       if (audioSourcePrefab == null)
       {
           Debug.LogError("SoundManager: audioSourcePrefab is not assigned.");
           enabled = false;
           return;
       }

       if (sources == null)
       {
           sources = new List<AudioSource>();
       }

       globalSource = GetComponent<AudioSource>();

       seaSource = InitializeSound(seaIdleSFX, seaIdleVolume, loop:true);
       engineSource = InitializeSound(engineSFX, 0f, loop:true);
       rainSource = InitializeSound(rainSFX, 0f, loop:true);
       thunderstormSource = InitializeSound(thunderstormSFX, 0f, loop:true);
       
       ShipUIController.OnEnginePowerChanged += HandleEnginePower;
       WeatherController.OnRain += HandleRain;
       WeatherController.OnThunderstorm += HandleThunderstorm;
       CheckCollision.OnCollision += HandlePlayerCollision;
    }

    void OnDestroy()
    {
        ShipUIController.OnEnginePowerChanged -= HandleEnginePower;
        WeatherController.OnRain -= HandleRain;
        WeatherController.OnThunderstorm -= HandleThunderstorm;
        CheckCollision.OnCollision -= HandlePlayerCollision;
    }

    private AudioSource CreateAudioSource()
    {
        GameObject newSourceGameObject = Instantiate(audioSourcePrefab, transform);
        AudioSource source = newSourceGameObject.GetComponent<AudioSource>();
        if (source == null)
        {
            source = newSourceGameObject.AddComponent<AudioSource>();
        }
        sources.Add(source);
        return source;
    }


    private AudioSource InitializeSound(AudioClip sound, float volume, bool loop = true) 
    {
        AudioSource source = CreateAudioSource();
        source.clip = sound;
        source.loop = loop;
        source.volume = volume;
        source.Play();

        return source;
    }

    private void HandleEnginePower(float newVolume)
    {
        engineSource.volume = newVolume;
    }

    private void HandleRain(bool rainState)
    {
        rainSource.volume = rainState ? rainVolume : 0f;
    }
    private void HandleThunderstorm(bool thunderstormState)
    {
        thunderstormSource.volume = thunderstormState ? thunderstormVolume : 0f;
    }

    private void HandlePlayerCollision()
    {
        //TODO: Fix RigidBody with obstacles
        globalSource.PlayOneShot(collisionSFX);
    }
}
