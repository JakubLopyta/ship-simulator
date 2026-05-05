using UnityEngine;
using UnityEngine.EventSystems;


public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    public Sprite obstacleSprite;
    public string obstacleName;

    Vector3 mousePosition;
    [SerializeField]
    public GameObject prefab;
    public GameObject draggedObject;
    public Transform obstaclesParent;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        GameObject container = GameObject.FindGameObjectWithTag("ObstaclesContainer");

        if (container != null)
        {
            obstaclesParent = container.transform;
        }
        else
        {
            Debug.LogWarning("Nie znaleziono obiektu z tagiem 'ObstaclesContainer' na scenie!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedObject = Instantiate(prefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, obstaclesParent);
        draggedObject.gameObject.tag = "Obstacle";

        Obstacle newObstacle = draggedObject.AddComponent<Obstacle>();
        newObstacle.obstacleName = string.IsNullOrEmpty(obstacleName) ? "Unknown obstacle" : obstacleName;
        newObstacle.obstacleObject = draggedObject;
        
        if (obstacleSprite != null)
        {
            newObstacle.sprite = obstacleSprite;
        }

        UpdateDraggedPosition(eventData);

        ObstaclesHandler.ObstaclesArray.Add(newObstacle);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            Obstacle obstacleComp = draggedObject.GetComponent<Obstacle>();
            if (obstacleComp != null)
            {
                obstacleComp.x = draggedObject.transform.position.x;
                obstacleComp.z = draggedObject.transform.position.z;
            }
        }
        draggedObject = null;
    }

    private void UpdateDraggedPosition(PointerEventData eventData)
    {
        if (draggedObject == null || mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            draggedObject.transform.position = hit.point;
        }
    }
}
