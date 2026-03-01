using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesHandler : MonoBehaviour
{
    [SerializeField]
    public float spawnProximity = 100f;
    public GameObject obstaclePrefab;
    public Sprite defaultSprite;

    public GameObject player;

    static public List<Obstacle> ObstaclesArray = new List<Obstacle>();
    static public List<Obstacle> visibleObstacles = new List<Obstacle>();


    public void checkObstaclesProximity()
    {
        if (ObstaclesArray.Count == 0) return;
        visibleObstacles.Clear();
        foreach (Obstacle obs in ObstaclesArray)
        {
            float distance = Vector3.Distance(player.transform.position, obs.obstacleObject.transform.position);
            if (distance < spawnProximity)
            {
                visibleObstacles.Add(obs);
                obs.obstacleObject.SetActive(true);
            }
            else
            {
                obs.obstacleObject.SetActive(false);
            }
        }
    }


    private void Update()
    {
        checkObstaclesProximity();
    }
}
