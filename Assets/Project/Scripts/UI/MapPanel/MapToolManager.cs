using System.Collections.Generic;
using UnityEngine;

public class MapToolManager : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Referencja do ScriptableObjecta nas³uchuj¹cego wejœcia.")]
    [SerializeField] private InputReader inputReader;

    [Header("Map Panel")]
    [Tooltip("G³ówny panel mapy, który chcemy w³¹czaæ/wy³¹czaæ.")]
    [SerializeField] private GameObject mapDisplayPanel;

    [Header("UI Panels")]
    [Tooltip("Panele odpowiadaj¹ce poszczególnym narzêdziom. Kolejnoœæ musi zgadzaæ siê z indeksami (np. 0 = Zaznaczanie).")]
    [SerializeField] private List<GameObject> toolUIPanels;

    [Header("Map Render Panel")]
    [Tooltip("Panel mapy po którym u¿ytkownik mo¿e siê poruszaæ click & drag.")]
    [SerializeField] private RectTransform mapArea;

    [SerializeField] private MapCameraController mapCameraController;

    private Dictionary<int, IMapTool> tools;
    private IMapTool currentTool;

    private bool isMapVisible = false;

    private bool wasMapClickedThisFrame = false;
    private Vector2 cachedMousePosition;

    private void Awake()
    {
        tools = new Dictionary<int, IMapTool>
        {
            { 0, new SelectionTool() }
            // { 1, new MeasureDistanceTool() }, 
            // { 2, new DrawPathTool() }
        };
    }

    private void Start()
    {
        SelectTool(0);
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnMapLeftClick += CacheMapClick;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnMapLeftClick -= CacheMapClick;
        }

        currentTool?.OnUnequip();
    }

    private void CacheMapClick(Vector2 screenPosition)
    {
        wasMapClickedThisFrame = true;
        cachedMousePosition = screenPosition;
    }

    private void Update()
    {
        if (wasMapClickedThisFrame)
        {
            wasMapClickedThisFrame = false;
            if (RectTransformUtility.RectangleContainsScreenPoint(mapArea, cachedMousePosition))
            {
                currentTool?.HandleClick(cachedMousePosition);
            }
            else
            {
                Debug.Log("Klikniêto poza map¹ - ignorujê.");
            }
        }
    }

    /// <summary>
    /// Tê metodê podpinasz pod przyciski UI w inspektorze (tzw. OnClick Event).
    /// Przekazujesz odpowiedni indeks (0 dla Zaznaczania, 1 dla Linijki itd.)
    /// </summary>
    public void SelectTool(int toolIndex)
    {
        if (tools.TryGetValue(toolIndex, out IMapTool newTool))
        {
            ChangeTool(newTool, toolIndex);
        }
        else
        {
            Debug.LogWarning($"Narzêdzie z indeksem {toolIndex} nie jest jeszcze zaimplementowane!");
        }
    }

    public void ToggleMapVisibility()
    {
        isMapVisible = !isMapVisible;

        if (inputReader != null)
        {
            inputReader.SetMapInputActive(isMapVisible);
        }

        if (mapCameraController != null)
        {
            mapCameraController.enabled = isMapVisible;
        }

        if (mapDisplayPanel != null)
        {
            mapDisplayPanel.SetActive(isMapVisible);
        }

        if (!isMapVisible && MapSelectionManager.Instance != null)
        {
            MapSelectionManager.Instance.ClearSelection();
        }

        Debug.Log($"Mapa jest teraz: {(isMapVisible ? "Widoczna" : "Ukryta")}");
    }

    private void ChangeTool(IMapTool newTool, int toolIndex)
    {
        if (currentTool == newTool) return;

        currentTool?.OnUnequip();

        currentTool = newTool;
        currentTool.OnEquip();

        UpdateUIPanels(toolIndex);
    }

    private void HandleMapClick(Vector2 screenPosition)
    {
        currentTool?.HandleClick(screenPosition);
    }

    private void UpdateUIPanels(int activeIndex)
    {
        for (int i = 0; i < toolUIPanels.Count; i++)
        {
            if (toolUIPanels[i] != null)
            {
                toolUIPanels[i].SetActive(i == activeIndex);
            }
        }
    }
}