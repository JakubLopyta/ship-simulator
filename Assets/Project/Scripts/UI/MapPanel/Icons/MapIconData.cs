using UnityEngine;

[CreateAssetMenu(fileName = "NewMapIcon", menuName = "Map Icon Data")]
public class MapIconData : ScriptableObject
{
    [Tooltip("Grafika wyœwietlana na mapie")]
    public Sprite IconSprite;

    [Tooltip("Kolor ikony (np. czerwony dla wroga, zielony dla gracza)")]
    public Color IconColor = Color.white;

    [Tooltip("Czy to jest strefa (Zone) o konkretnym promieniu?")]
    public bool IsZoneType = false;

    [Tooltip("Domyœlny promieñ (jeœli obiekt sam go nie nadpisze)")]
    public float DefaultRadius = 10f;
}
