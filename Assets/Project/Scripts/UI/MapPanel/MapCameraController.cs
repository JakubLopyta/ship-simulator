using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Camera mapCamera;

    [Header("Pan (Przesuwanie) Settings")]
    [SerializeField] private float panSpeed = 0.5f;
    [Tooltip("Wiêksza wartoœæ = szybsze przesuwanie kamery po oddaleniu (zoomie)")]
    [SerializeField] private float panZoomMultiplier = 0.05f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 200f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 1000f;

    private float targetZoom;

    private void Start()
    {
        if (mapCamera == null) mapCamera = GetComponent<Camera>();

        if (mapCamera != null)
        {
            targetZoom = mapCamera.orthographicSize;
        }
    }

    private void LateUpdate()
    {
        if (inputReader == null || mapCamera == null) return;

        HandleZoom();
        HandlePan();
    }

    private void HandleZoom()
    {
        float scrollInput = inputReader.MapZoomDelta;

        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        if (Mathf.Abs(mapCamera.orthographicSize - targetZoom) > 0.01f)
        {
            mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
    }

    private void HandlePan()
    {
        if (inputReader.IsMapDragPressed)
        {
            Vector2 mouseDelta = inputReader.MapDragDelta;

            if (mouseDelta.sqrMagnitude > 0)
            {
                float currentZoomFactor = mapCamera.orthographicSize * panZoomMultiplier;

                Vector3 moveTranslation = new Vector3(
                    -mouseDelta.x * panSpeed * currentZoomFactor,
                    0f,
                    -mouseDelta.y * panSpeed * currentZoomFactor
                );

                mapCamera.transform.Translate(moveTranslation, Space.World);
            }
        }
    }
}