using System;
using UnityEngine;


public class MapSelectionManager : MonoBehaviour
{
    public static MapSelectionManager Instance { get; private set; }

    public Ship SelectedShip { get; private set; }

    public event Action<Ship> OnShipSelected;
    public event Action OnShipDeselected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectShip(Ship ship)
    {
        if (SelectedShip == ship) return;

        SelectedShip = ship;
        Debug.Log($"[MapSelectionManager] Zaznaczono statek: {ship.gameObject.name}");

        OnShipSelected?.Invoke(ship);
    }

    public void ClearSelection()
    {
        if (SelectedShip == null) return;

        SelectedShip = null;
        Debug.Log("[MapSelectionManager] Odznaczono statek.");

        OnShipDeselected?.Invoke();
    }
}
