using TMPro;
using UnityEngine;

public class SelectedShipDataPanel : MonoBehaviour
{
    [Header("General Information Section")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text banderaText;
    [SerializeField] private TMP_Text imoText;
    [SerializeField] private TMP_Text mmsiText;
    [SerializeField] private TMP_Text callsignText;

    private void Start()
    {
        if (MapSelectionManager.Instance != null)
        {
            MapSelectionManager.Instance.OnShipSelected += Display;
            MapSelectionManager.Instance.OnShipDeselected += Clear;
        }

        Clear();
    }

    private void OnDestroy()
    {
        if (MapSelectionManager.Instance != null)
        {
            MapSelectionManager.Instance.OnShipSelected -= Display;
            MapSelectionManager.Instance.OnShipDeselected -= Clear;
        }
    }

    private void Display(Ship data)
    {
        if (data == null) return;

        nameText.text = data.name;

        // Tutaj wyci¹gasz dane z obiektu (zostawi³em tak, jak mia³eœ przygotowane)
        typeText.text = "Commercial Ship";
        banderaText.text = "Poland";
        imoText.text = "IMO 1234567";
        mmsiText.text = "261000000";
        callsignText.text = "SNBH";
    }

    private void Clear()
    {
        nameText.text = "---";
        typeText.text = "---";
        banderaText.text = "---";
        imoText.text = "---";
        mmsiText.text = "---";
        callsignText.text = "---";
    }
}