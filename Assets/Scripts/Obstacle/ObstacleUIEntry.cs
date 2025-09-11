using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObstacleUIEntry : MonoBehaviour
{
    public Image obstacleImage;
    public TextMeshProUGUI indexText;
    public TextMeshProUGUI obstacleNameText;

    public void Setup(Sprite sprite, string name, int index)
    {
        obstacleImage.sprite = sprite;
        obstacleNameText.text = name;
        indexText.text = index.ToString();

        obstacleImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
    }
}