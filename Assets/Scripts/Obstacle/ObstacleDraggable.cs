using UnityEngine;
using UnityEngine.EventSystems;


public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Sprite obstacleSprite;


    Vector3 mousePosition;
    public GameObject prefab;
    public GameObject draggedObject;

    // Implement required interface methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedObject = Instantiate(prefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity);
        draggedObject.gameObject.tag = "Obstacle";
        Obstacle newObstacle = draggedObject.AddComponent<Obstacle>();
        newObstacle.x = draggedObject.transform.position.x;
        newObstacle.z = draggedObject.transform.position.z;
        newObstacle.obstacleName = "Buoy";
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
