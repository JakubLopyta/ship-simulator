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

    public Button addObstacleButton;
    public TMP_InputField obstacleNameField;
    public TMP_InputField xField;
    public TMP_InputField zField;
    
    public void AddObstacle()
    {
        string name = string.IsNullOrEmpty(obstacleNameField.text) ? "New Obstacle" : obstacleNameField.text;
        float x = 0; 
        float z = 0;

        float.TryParse(xField.text, out x);
        float.TryParse(zField.text, out z);

        GameObject newObstacleObject = Instantiate(obstaclePrefab, new Vector3(x, 0f, z), Quaternion.identity);

        Obstacle newObstacle = newObstacleObject.AddComponent<Obstacle>();
        newObstacle.gameObject.tag = "Obstacle";
        newObstacle.x = x;
        newObstacle.z = z;
        newObstacle.obstacleName = name;
        newObstacle.sprite = defaultSprite;
        newObstacle.obstacleObject = newObstacleObject;

        ObstaclesArray.Add(newObstacle);
    }

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

    private void Start()
    {
        Button addObButton = addObstacleButton.GetComponent<Button>();
        addObButton.onClick.AddListener(AddObstacle);
    }

    private void Update()
    {
        checkObstaclesProximity();
    }
}
