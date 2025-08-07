using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipSpawner : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown shipDropdown;
    public TMP_InputField nameInput;
    public TMP_InputField callSignInput;
    public TMP_InputField mmsiInput;
    public TMP_InputField posXInput;
    public TMP_InputField posYInput;
    public TMP_InputField hdgInput;
    public Button submitButton;

    [Header("Ship Prefabs")]
    public GameObject[] shipPrefabs;

    private void Start()
    {
        submitButton.onClick.AddListener(SpawnShip);

        PopulateDropdown();
    }

    void SpawnShip()
    {
        int selectedIndex = shipDropdown.value;

        if (selectedIndex < 0 || selectedIndex >= shipPrefabs.Length)
        {
            Debug.LogWarning("Invalid dropdown selection.");
            return;
        }

        // Validate and parse inputs
        if (!int.TryParse(mmsiInput.text, out int mmsi) ||
            !float.TryParse(posXInput.text, out float posX) ||
            !float.TryParse(posYInput.text, out float posY) ||
            !float.TryParse(hdgInput.text, out float heading))
        {
            Debug.LogWarning("Invalid MMSI, position, or heading input.");
            return;
        }

        string shipName = nameInput.text;
        string callSign = callSignInput.text;

        // Instantiate the ship prefab
        GameObject shipGO = Instantiate(
            shipPrefabs[selectedIndex],
            new Vector3(posX, 0f, posY), // Adjust as needed
            Quaternion.Euler(0f, heading, 0f)
        );

        // Assign values to Ship component
        Ship ship = shipGO.GetComponent<Ship>();
        if (ship != null)
        {
            ship.Name = shipName;
            ship.CallSign = callSign;
            ship.MMSI = mmsi;

            ship.PosX = posX;
            ship.PosY = posY;
            ship.Hdg = heading;
        }
        else
        {
            Debug.LogWarning("Spawned prefab is missing the Ship component.");
        }
    }
    private void PopulateDropdown()
    {
        shipDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();

        foreach (GameObject prefab in shipPrefabs)
        {
            if (prefab != null)
                options.Add(prefab.name); // You could also get a custom name if needed
            else
                options.Add("Unnamed Ship");
        }

        shipDropdown.AddOptions(options);
    }

}
