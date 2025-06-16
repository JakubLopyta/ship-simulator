using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class ShipControlPanel : MonoBehaviour
{
    public GameObject panel;
    public Button topbarButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button topbarBtn = topbarButton.GetComponent<Button>();
        topbarBtn.onClick.AddListener(onTopbarButtonClick);
    }

    void onTopbarButtonClick()
    {
        panel.SetActive(!panel.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
