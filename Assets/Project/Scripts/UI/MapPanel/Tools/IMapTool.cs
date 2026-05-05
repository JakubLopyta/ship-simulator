using UnityEngine;

public interface IMapTool
{
    public void OnEquip();

    public void OnUnequip();

    public void HandleClick(Vector2 position);
}
