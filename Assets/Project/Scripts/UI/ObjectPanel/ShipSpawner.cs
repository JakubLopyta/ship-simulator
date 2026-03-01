using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipSpawner : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown shipDropdown;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField callSignInput;
    [SerializeField] private TMP_InputField mmsiInput;
    [SerializeField] private TMP_InputField posXInput;
    [SerializeField] private TMP_InputField posYInput;
    [SerializeField] private TMP_InputField hdgInput;
    [SerializeField] private Button submitButton;

    [Header("Ship Prefabs")]
    [SerializeField] private GameObject[] shipPrefabs;

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SpawnShip);
        }

        PopulateDropdown();
    }

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(SpawnShip);
        }
    }

    private void SpawnShip()
    {
        if (shipDropdown == null || shipPrefabs == null)
        {
            Debug.LogWarning("ShipSpawner is missing required references.");
            return;
        }

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
            ship.ShipName = shipName;
            ship.MMSI = mmsi;

			ship.transform.position = new Vector3((float)posX, 0f, (float)posY);
			ship.Hdg = heading;
        }
        else
        {
            Debug.LogWarning("Spawned prefab is missing the Ship component.");
        }
    }
    private void PopulateDropdown()
    {
        if (shipDropdown == null || shipPrefabs == null)
        {
            return;
        }

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
