using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipListUIManager : MonoBehaviour
{
    public GameObject shipRowPrefab;              // Prefab ShipRow
    public Transform contentPanel;                // Content z ScrollView

    // Mapa: Ship GameObject => odpowiadaj¹cy ShipRow
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
            yield return new WaitForSeconds(1f);  // co sekundê
        }
    }

    void UpdateShipRows()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");

        HashSet<GameObject> currentShips = new HashSet<GameObject>(ships);

        // 1. Dodaj nowe statki
        foreach (GameObject ship in ships)
        {
            if (!shipRows.ContainsKey(ship))
            {
                GameObject rowGO = Instantiate(shipRowPrefab, contentPanel);
                ShipRow row = rowGO.GetComponent<ShipRow>();
                row.SetShipData(ship);
                shipRows[ship] = row;
            }
        }

        // 2. Aktualizuj dane
        foreach (var kvp in shipRows)
        {
            GameObject ship = kvp.Key;
            ShipRow row = kvp.Value;

            if (ship != null)
                row.SetShipData(ship);
        }

        // 3. Usuñ wiersze dla nieistniej¹cych statków
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
