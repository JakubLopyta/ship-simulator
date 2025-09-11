using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesListGenerator : MonoBehaviour
{
    public Transform obstaclesPanel;
    public GameObject obstacleEntryPrefab;
    private List<GameObject> currentEntries = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateObstacleList();
    }

    // Update is called once per frame
    void Update()
    {
        updateObstacleList();
    }
    void updateObstacleList()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles.Length == currentEntries.Count) return;

        foreach (var obstacle in currentEntries)
        {
            Destroy(obstacle);
        }
        currentEntries.Clear();


                foreach (var obstacle in obstacles)
                {
                    GameObject newObstacleEntry = Instantiate(obstacleEntryPrefab, obstaclesPanel);
                    Obstacle data = obstacle.GetComponent<Obstacle>();
                    if (data != null)
                    {
                        ObstacleUIEntry uiEntry = newObstacleEntry.GetComponent<ObstacleUIEntry>();
                        uiEntry.Setup(data.sprite, data.obstacleName, currentEntries.Count + 1);
                       
                    }
                    currentEntries.Add(newObstacleEntry);
                }


    }
}
