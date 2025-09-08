using System.Collections;
using UnityEngine;

public class WeatherPlayerFollower : MonoBehaviour
{
    public GameObject player;
    public GameObject fogParticles;
    public GameObject rainParticles;
    public GameObject thunderstormParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
            if (player != null)
            {
                Vector3 playerPosition = player.transform.position;
                if (fogParticles != null)
                    fogParticles.transform.position = playerPosition;
                if (rainParticles != null)
                    rainParticles.transform.position = playerPosition;
                if (thunderstormParticles != null)
                    thunderstormParticles.transform.position = playerPosition;
            }
    }
    
}
