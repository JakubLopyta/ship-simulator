using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionTool : IMapTool
{
    public void OnEquip()
    {
        Debug.Log("Wybrano narzêdzie zaznaczania.");
    }

    public void OnUnequip()
    {
        Debug.Log("Od³o¿ono narzêdzie zaznaczania.");
    }

    public void HandleClick(Vector2 mousePosition)
    {
        if (MapSelectionManager.Instance == null) return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            MapIconClicker icon = result.gameObject.GetComponent<MapIconClicker>();

            if (icon != null && icon.worldTarget != null)
            {
                Ship ship = icon.worldTarget.GetComponent<Ship>();

                if (ship != null)
                {
                    MapSelectionManager.Instance.SelectShip(ship);
                    return;
                }
            }

            MapSelectionManager.Instance.ClearSelection();
        }
    }
}