using UnityEngine;

public class HidePreviewSea : MonoBehaviour
{
    public GameObject seaPreview;
    // The main purpose of this script is to hide placeholder sea outside Unity Preview
    void Start()
    {
        seaPreview.SetActive(false);
    }

}
