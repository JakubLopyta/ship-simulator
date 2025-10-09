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

    void Start()
    {
        obstaclesParent = GameObject.FindGameObjectWithTag("ObstaclesContainer").transform;
    }

    // Implement required interface methods
    public void OnBeginDrag(PointerEventData eventData)
    {

        draggedObject = Instantiate(prefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, obstaclesParent);
        draggedObject.gameObject.tag = "Obstacle";
        Obstacle newObstacle = draggedObject.AddComponent<Obstacle>();
        newObstacle.x = draggedObject.transform.position.x;
        newObstacle.z = draggedObject.transform.position.z;
        newObstacle.obstacleName = "Unknown obstacle";
        if (obstacleName != null)
        {
            newObstacle.obstacleName = obstacleName;
        }
        
        newObstacle.obstacleObject = draggedObject;
        if (obstacleSprite != null)
        {
            newObstacle.sprite = obstacleSprite;
        }

        ObstaclesHandler.ObstaclesArray.Add(newObstacle);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Specify the type argument explicitly, e.g., Transform
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            draggedObject.transform.position = hit.point;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        
    }


}
