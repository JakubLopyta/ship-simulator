using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

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
    [SerializeField] private float topViewMinDistance = 100f;
    [SerializeField] private float topViewMaxDistance = 2000f;
    [SerializeField] private float topViewZoomStep = 300f;
    [SerializeField] private float topViewZoomSpeed = 500f;

    private float x = 0f;
    private float y = 0f;

    private bool isTopView = false;
    private float savedX, savedY, savedDistance;
    private Vector3 topViewPosition;

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
        OriginManager.OnWorldRecentered += HandleWorldRecentered;
    }

    void OnDisable()
    {
        ToolbarUIController.OnZoomIn -= HandleZoomIn;
        ToolbarUIController.OnZoomOut -= HandleZoomOut;
        ToolbarUIController.OnMove -= HandleToggleTopView;
        OriginManager.OnWorldRecentered -= HandleWorldRecentered;
    }

    private void HandleZoomIn(bool _)
    {
        if (isTopView)
        {
            topViewPosition.y = Mathf.Clamp(topViewPosition.y - topViewZoomStep, topViewMinDistance, topViewMaxDistance);
            return;
        }
        distance = Mathf.Clamp(distance - zoomStep, minDistance, maxDistance);
    }

    private void HandleZoomOut(bool _)
    {
        if (isTopView)
        {
            topViewPosition.y = Mathf.Clamp(topViewPosition.y + topViewZoomStep, topViewMinDistance, topViewMaxDistance);
            return;
        }
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

            x = 0f;
            y = 90f;
            distance = topViewDistance;
            topViewPosition = target.position + Vector3.up * topViewDistance;
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

        if (!isTopView && inputReader.IsRightMouseButtonPressed)
        {
            x += inputReader.LookDelta.x * xSpeed * Time.unscaledDeltaTime;
            y -= inputReader.LookDelta.y * ySpeed * Time.unscaledDeltaTime;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        }

        float scroll = inputReader.ScrollDelta;
        if (isTopView)
            topViewPosition.y = Mathf.Clamp(topViewPosition.y - scroll * topViewZoomSpeed, topViewMinDistance, topViewMaxDistance);
        else
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        if (isTopView)
        {
            if (inputReader.IsRightMouseButtonPressed)
            {
                float panSpeed = topViewPosition.y * 0.001f;
                topViewPosition.x -= inputReader.LookDelta.x * panSpeed * xSpeed;
                topViewPosition.z -= inputReader.LookDelta.y * panSpeed * ySpeed;
            }

            transform.SetPositionAndRotation(topViewPosition, Quaternion.Euler(90f, 0f, 0f));
            return;
        }

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.position;
        transform.SetPositionAndRotation(position, rotation);
    }

    private void HandleWorldRecentered(Vector3 offset)
    {
        if (isTopView)
            topViewPosition += offset;
    }
}
