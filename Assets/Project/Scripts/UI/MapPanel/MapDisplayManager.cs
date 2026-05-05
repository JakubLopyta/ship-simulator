using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mapCamera;
    [SerializeField] private RectTransform mapContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject pointIconPrefab;
    [SerializeField] private GameObject zoneIconPrefab;

    private class IconInstance
    {
        public MapEntity Entity;
        public RectTransform RectTrans;
        public GameObject GameObject;
    }

    private readonly List<IconInstance> activeIcons = new List<IconInstance>();

    private void Start()
    {
        if (MapEntityManager.Instance != null)
        {
            MapEntityManager.Instance.OnEntityAdded += AddIcon;
            MapEntityManager.Instance.OnEntityRemoved += RemoveIcon;

            foreach (var entity in MapEntityManager.Instance.ActiveEntities)
            {
                AddIcon(entity);
            }
        }
    }

    private void OnDestroy()
    {
        if (MapEntityManager.Instance != null)
        {
            MapEntityManager.Instance.OnEntityAdded -= AddIcon;
            MapEntityManager.Instance.OnEntityRemoved -= RemoveIcon;
        }
    }

    private void AddIcon(MapEntity entity)
    {
        if (entity.IconConfig == null) return;

        GameObject prefabToUse = entity.IconConfig.IsZoneType ? zoneIconPrefab : pointIconPrefab;
        GameObject iconGO = Instantiate(prefabToUse, mapContainer);

        Image img = iconGO.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = entity.IconConfig.IconSprite;
            img.color = entity.IconConfig.IconColor;
        }
        else
        {
            Debug.LogWarning($"[MapDisplayManager] Uwaga! Twój prefabrykat {prefabToUse.name} nie posiada komponentu Image. Nie mogê podmieniæ ikony!");
        }

        MapIconClicker clicker = iconGO.GetComponent<MapIconClicker>();
        if (clicker == null) clicker = iconGO.AddComponent<MapIconClicker>();
        clicker.worldTarget = entity.transform;

        activeIcons.Add(new IconInstance
        {
            Entity = entity,
            RectTrans = iconGO.GetComponent<RectTransform>(),
            GameObject = iconGO
        });
    }

    private void RemoveIcon(MapEntity entity)
    {
        int index = activeIcons.FindIndex(i => i.Entity == entity);
        if (index >= 0)
        {
            Destroy(activeIcons[index].GameObject);
            activeIcons.RemoveAt(index);
        }
    }

    private void LateUpdate()
    {
        if (mapCamera == null || activeIcons.Count == 0) return;

        Vector2 mapSize = mapContainer.rect.size;
        Rect visibilityRect = new Rect(-mapSize.x / 2, -mapSize.y / 2, mapSize.x, mapSize.y);

        for (int i = 0; i < activeIcons.Count; i++)
        {
            IconInstance icon = activeIcons[i];

            if (icon.Entity == null) continue;

            Vector3 viewportPos = mapCamera.WorldToViewportPoint(icon.Entity.transform.position);

            bool isBehindCamera = viewportPos.z <= 0;

            Vector2 anchoredPos = new(
                (viewportPos.x - 0.5f) * mapSize.x,
                (viewportPos.y - 0.5f) * mapSize.y
                );

            bool isInsidePanel = visibilityRect.Contains(anchoredPos, true);
            bool shouldBeVisible = !isBehindCamera && isInsidePanel;

            bool isVisible = viewportPos.z > 0 &&
                             viewportPos.x >= -0.0f && viewportPos.x <= 1.0f &&
                             viewportPos.y >= -0.0f && viewportPos.y <= 1.0f;

            if (icon.GameObject.activeSelf != isVisible)
            {
                icon.GameObject.SetActive(isVisible);
            }

            if (isVisible)
            {
                icon.RectTrans.anchoredPosition = new Vector2(
                    (viewportPos.x - 0.5f) * mapSize.x,
                    (viewportPos.y - 0.5f) * mapSize.y
                );

                if (icon.Entity.IconConfig.IsZoneType)
                {
                    float screenRadius = (icon.Entity.GetRadius() / (mapCamera.orthographicSize * 2f)) * mapSize.y;
                    icon.RectTrans.sizeDelta = new Vector2(screenRadius * 2f, screenRadius * 2f);
                }
            }
        }
    }
}