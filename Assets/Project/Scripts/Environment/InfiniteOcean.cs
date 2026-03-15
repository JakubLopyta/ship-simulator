using UnityEngine;

public class InfiniteOcean : MonoBehaviour
{
    private Transform shipTransform;

    private void Start()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        if (ship != null)
            shipTransform = ship.transform;
        else
            Debug.LogWarning("InfiniteOcean: No GameObject with tag 'Ship' found.");
    }

    private void LateUpdate()
    {
        if (shipTransform == null) return;

        Vector3 pos = shipTransform.position;
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
    }
}
