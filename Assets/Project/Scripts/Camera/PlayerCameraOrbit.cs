using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 100f;
    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float zoomStep = 150f;
    [SerializeField] private float minDistance = 250f;
    [SerializeField] private float maxDistance = 1000f;

    [SerializeField] private float xSpeed = 120f;
    [SerializeField] private float ySpeed = 80f;

    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;

    [Header("Top View")]
    [SerializeField] private float topViewDistance = 500f;

    private float x = 0f;
    private float y = 0f;

    private bool isTopView = false;
    private float savedX, savedY, savedDistance;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void OnEnable()
    {
        ToolbarUIController.OnZoomIn += HandleZoomIn;
        ToolbarUIController.OnZoomOut += HandleZoomOut;
        ToolbarUIController.OnMove += HandleToggleTopView;
    }

    void OnDisable()
    {
        ToolbarUIController.OnZoomIn -= HandleZoomIn;
        ToolbarUIController.OnZoomOut -= HandleZoomOut;
        ToolbarUIController.OnMove -= HandleToggleTopView;
    }

    private void HandleZoomIn(bool _)
    {
        if (isTopView) return;
        distance = Mathf.Clamp(distance - zoomStep, minDistance, maxDistance);
    }

    private void HandleZoomOut(bool _)
    {
        if (isTopView) return;
        distance = Mathf.Clamp(distance + zoomStep, minDistance, maxDistance);
    }

    private void HandleToggleTopView(bool _)
    {
        isTopView = !isTopView;

        if (isTopView)
        {
            savedX = x;
            savedY = y;
            savedDistance = distance;

            y = 90f;
            distance = topViewDistance;
        }
        else
        {
            x = savedX;
            y = savedY;
            distance = savedDistance;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (!isTopView && Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.unscaledDeltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.unscaledDeltaTime;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        }

        if (!isTopView)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
        }

        Quaternion rotation = isTopView
            ? Quaternion.Euler(90f, x, 0f)
            : Quaternion.Euler(y, x, 0);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
