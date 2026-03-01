using UnityEngine;

public class WeatherPlayerFollower : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fogParticles;
    [SerializeField] private GameObject rainParticles;
    [SerializeField] private GameObject thunderstormParticles;
    void Update()
    {
            if (player != null)
            {
                Vector3 playerPosition = player.transform.position;
                if (fogParticles != null)
                    fogParticles.transform.position = playerPosition;
                if (rainParticles != null)
                    rainParticles.transform.position = new Vector3(playerPosition.x, playerPosition.y + 5, playerPosition.z);
                if (thunderstormParticles != null)
                    thunderstormParticles.transform.position = playerPosition;
            }
    }
    
}
