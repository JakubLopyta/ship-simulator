using TMPro;
using UnityEngine;

public class ShipRow : MonoBehaviour
{
    public TextMeshProUGUI shipNameText;
    public TextMeshProUGUI shipTypeText;
    public TextMeshProUGUI positionXText;
    public TextMeshProUGUI positionZText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI headingText;

    public void SetShipData(GameObject shipObj)
    {
        Ship ship = shipObj.GetComponent<Ship>();
        if (ship == null) return;

        shipNameText.text = ship.Name;
        
        // ShipType = nazwa prefabrykatu, bez "(Clone)"
        string prefabName = shipObj.name.Replace("(Clone)", "").Trim();
        shipTypeText.text = prefabName;

        Vector3 pos = ship.transform.position;
        positionXText.text = pos.x.ToString("F1");
        positionZText.text = pos.z.ToString("F1");

        speedText.text = ship.Speed.ToString("F1") + " kts";

        headingText.text = ship.Hdg.ToString("F0") + "°";
    }
}
