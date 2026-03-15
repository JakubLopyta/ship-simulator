using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ShipTrajectoryTracer : MonoBehaviour
{
    [SerializeField] private Color trailColor = Color.red;
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private float minDistanceToRecord = 2f;

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();
    private bool isTracking = false;
    private Transform shipTransform;

    private void Awake()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        if (ship != null)
            shipTransform = ship.transform;
        else
            Debug.LogWarning("ShipTrajectoryTracer: No GameObject with tag 'Ship' found.");

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
        isTracking = true;
    }

    private void OnEnable()
    {
        ToolbarUIController.OnLine += HandleToggle;
        ToolbarUIController.OnRestart += HandleRestart;
        ToolbarUIController.OnStop += HandleStop;
        OriginManager.OnWorldRecentered += HandleWorldRecentered;
        Camera.onPreCull += HandlePreCull;
    }

    private void OnDisable()
    {
        ToolbarUIController.OnLine -= HandleToggle;
        ToolbarUIController.OnRestart -= HandleRestart;
        ToolbarUIController.OnStop -= HandleStop;
        OriginManager.OnWorldRecentered -= HandleWorldRecentered;
        Camera.onPreCull -= HandlePreCull;
    }

    private void OnValidate()
    {
        if (lineRenderer == null) return;
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    private void Update()
    {
        if (!isTracking || shipTransform == null) return;

        Vector3 currentPos = shipTransform.position + Vector3.up * 0.5f;

        if (positions.Count == 0 ||
            Vector3.Distance(currentPos, positions[positions.Count - 1]) >= minDistanceToRecord)
        {
            positions.Add(currentPos);
            RefreshLine();
        }
    }

    private void HandlePreCull(Camera _)
    {
        if (positions.Count > 1)
            UpdateBounds();
    }

    private void HandleWorldRecentered(Vector3 offset)
    {
        for (int i = 0; i < positions.Count; i++)
            positions[i] += offset;

        RefreshLine();
    }

    private void RefreshLine()
    {
        if (positions.Count == 0) return;

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        Vector3 min = positions[0];
        Vector3 max = positions[0];
        foreach (Vector3 p in positions)
        {
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }
        lineRenderer.bounds = new Bounds((min + max) * 0.5f, max - min + Vector3.one * lineWidth * 2f);
    }

    private void HandleToggle(bool active)
    {
        lineRenderer.enabled = active;
    }

    private void HandleRestart(bool _)
    {
        ClearTrail();
    }

    private void HandleStop(bool _)
    {
        // tracking continues, only simulation stops
    }

    private void ClearTrail()
    {
        positions.Clear();
        lineRenderer.positionCount = 0;
    }
}
