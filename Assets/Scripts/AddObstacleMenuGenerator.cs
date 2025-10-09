using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class AddObstacleMenuGenerator : MonoBehaviour
{
    XmlDocument obstacleDataXML;
    public Transform obstaclesPanel;
    public GameObject obstaclesPreviewButtonPrefab;

    private void Awake()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/obstacles");
        obstacleDataXML = new XmlDocument();
        obstacleDataXML.LoadXml(xmlTextAsset.text);
        FindAllObstacles();

    }

    public void FindAllObstacles()
    {
        XmlNodeList obstacles = obstacleDataXML.SelectNodes("/Obstacles/Obstacle");

        foreach (Transform child in obstaclesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (XmlNode obstacle in obstacles)
        {
            GameObject newObstacleEntry = Instantiate(obstaclesPreviewButtonPrefab, obstaclesPanel);
            Draggable draggable = newObstacleEntry.GetComponent<Draggable>();
            ObstacleUIEntry uiEntry = newObstacleEntry.GetComponent<ObstacleUIEntry>();

            XmlNode titleNode = obstacle.SelectSingleNode("Title");
            if (titleNode != null)
            {
                draggable.obstacleName = titleNode.InnerText;
            }

            XmlNode imageNode = obstacle.SelectSingleNode("Image");
            if (imageNode != null)
            {
                Sprite sprite = Resources.Load<Sprite>("Sprites/Obstacles/" + imageNode.InnerText.Trim());
                if (sprite != null)
                {
                    uiEntry.obstacleImage.sprite = sprite;
                    uiEntry.obstacleImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    draggable.obstacleSprite = sprite;
                }
                else
                {
                    Debug.LogWarning("Sprite not found for: " + imageNode.InnerText);
                }
            }
            else
            {
                Debug.LogWarning("Image node missing in obstacle XML.");
            }
            XmlNode prefabNode = obstacle.SelectSingleNode("Prefab");
            if (prefabNode != null)
            {
                GameObject loadedPrefab = Resources.Load<GameObject>($"Prefabs/Obstacles/{prefabNode.InnerText.Trim()}");
                if (loadedPrefab != null)
                {
                    draggable.prefab = loadedPrefab;
                }
                else
                {
                    Debug.LogError($"Prefab not found at Resources/Prefabs/Obstacles/{prefabNode.InnerText.Trim()}");
                }
            }

        }
    }
}
