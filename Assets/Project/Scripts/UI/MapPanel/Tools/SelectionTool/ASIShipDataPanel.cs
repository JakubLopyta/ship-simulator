using TMPro;
using UnityEngine;

public class ASIShipDataPanel : MonoBehaviour
{
    [Header("Position and Movement Section")]
    [SerializeField] private TMP_Text HDG;
    [SerializeField] private TMP_Text ROT;
    [SerializeField] private TMP_Text COG;
    [SerializeField] private TMP_Text SOG;
    [SerializeField] private TMP_Text Speed;
    [SerializeField] private TMP_Text Power;
    [SerializeField] private TMP_Text Latitude;
    [SerializeField] private TMP_Text Longitude;

    private Ship currentData;

    private bool isTracking = false;

    private void Start()
    {
        if (MapSelectionManager.Instance != null)
        {
            MapSelectionManager.Instance.OnShipSelected += StartTracking;
            MapSelectionManager.Instance.OnShipDeselected += Clear;
        }

        Clear();
    }

    private void OnDestroy()
    {
        if (MapSelectionManager.Instance != null)
        {
            MapSelectionManager.Instance.OnShipSelected -= StartTracking;
            MapSelectionManager.Instance.OnShipDeselected -= Clear;
        }
    }

    private void StartTracking(Ship data)
    {
        if (data == null) return;

        currentData = data;
        isTracking = true;

        DisplayCurrentData();
    }

    private void Clear()
    {
        isTracking = false;
        currentData = null;

        HDG.text = "---°";
        ROT.text = "---°/min";
        COG.text = "---°";
        SOG.text = "--- kn";
        Speed.text = "--- kn";
        Power.text = "--- %";
        Latitude.text = "---";
        Longitude.text = "---";
    }

    private void Update()
    {
        if (!isTracking || currentData == null) return;

        DisplayCurrentData();
    }

    private void DisplayCurrentData()
    {
        HDG.text = $"{currentData.Hdg:F0}°";
        ROT.text = $"{currentData.Rot:F1}°/min";
        COG.text = $"{currentData.Cog:F0}°";
        SOG.text = $"{currentData.Sog:F1} kn";
        Speed.text = $"{currentData.Speed:F1} kn";
        Power.text = $"{currentData.EnginePower:F0} %";
        Latitude.text = $"{currentData.LatitudeDeg:F4}";
        Longitude.text = $"{currentData.LongitudeDeg:F4}";
    }
}