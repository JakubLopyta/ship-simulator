using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipListUIManager : MonoBehaviour
{
    public GameObject shipRowPrefab;
    public Transform contentPanel;
    public ShipUIController shipUIController;

    private Dictionary<GameObject, ShipRow> shipRows = new Dictionary<GameObject, ShipRow>();

    void Start()
    {
        StartCoroutine(RefreshShipList());
    }

    IEnumerator RefreshShipList()
    {
        while (true)
        {
            UpdateShipRows();
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateShipRows()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");

        HashSet<GameObject> currentShips = new HashSet<GameObject>(ships);

        foreach (GameObject ship in ships)
        {
            if (!shipRows.ContainsKey(ship))
            {
                GameObject rowGO = Instantiate(shipRowPrefab, contentPanel);
                ShipRow row = rowGO.GetComponent<ShipRow>();
                row.SetShipData(ship);
                row.SetUIController(shipUIController);
                shipRows[ship] = row;
            }
        }

        foreach (var kvp in shipRows)
        {
            GameObject ship = kvp.Key;
            ShipRow row = kvp.Value;

            if (ship != null)
                row.SetShipData(ship);
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (var kvp in shipRows)
        {
            if (!currentShips.Contains(kvp.Key) || kvp.Key == null)
            {
                Destroy(kvp.Value.gameObject);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var ship in toRemove)
        {
            shipRows.Remove(ship);
        }
    }
}
