using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class MapEntityManager : MonoBehaviour
{
    public static MapEntityManager Instance { get; private set; }

    // Lista wszystkich aktywnych obiektów w œwiecie gry
    public List<MapEntity> ActiveEntities { get; private set; } = new List<MapEntity>();

    // Sygna³y dla Mened¿era Wyœwietlania
    public event Action<MapEntity> OnEntityAdded;
    public event Action<MapEntity> OnEntityRemoved;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(MapEntity entity)
    {
        if (!ActiveEntities.Contains(entity))
        {
            ActiveEntities.Add(entity);
            OnEntityAdded?.Invoke(entity);
        }
    }

    public void Unregister(MapEntity entity)
    {
        if (ActiveEntities.Contains(entity))
        {
            ActiveEntities.Remove(entity);
            OnEntityRemoved?.Invoke(entity);
        }
    }
}