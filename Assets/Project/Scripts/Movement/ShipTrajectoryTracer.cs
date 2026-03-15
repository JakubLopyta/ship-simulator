using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ShipTrajectoryTracer : MonoBehaviour
{
    [SerializeField] private Color lineColor = Color.red;
    [SerializeField] private float lineWidth = 3f;
    [SerializeField] private float recordInterval = 1f;
    [SerializeField] private float heightAboveWater = 30f;

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new();
    private Transform shipTransform;
    private bool simulationRunning = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        lineRenderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 1e6f);

        GameObject shipObj = GameObject.FindGameObjectWithTag("Ship");
        if (shipObj != null)
            shipTransform = shipObj.transform;
        else
            Debug.LogWarning("ShipTrajectoryTracer: no GameObject with tag 'Ship' found.");
    }

    private void OnEnable()
    {
        ToolbarUIController.OnLine += SetVisible;
        ToolbarUIController.OnPlay += OnPlay;
        ToolbarUIController.OnPause += OnPause;
        ToolbarUIController.OnStop += OnStop;
        ToolbarUIController.OnRestart += OnRestart;
        OriginManager.OnWorldRecentered += OnWorldRecentered;
    }

    private void OnDisable()
    {
        ToolbarUIController.OnLine -= SetVisible;
        ToolbarUIController.OnPlay -= OnPlay;
        ToolbarUIController.OnPause -= OnPause;
        ToolbarUIController.OnStop -= OnStop;
        ToolbarUIController.OnRestart -= OnRestart;
        OriginManager.OnWorldRecentered -= OnWorldRecentered;
    }

    private void OnPlay(bool _) => simulationRunning = true;
    private void OnPause(bool _) => simulationRunning = false;
    private void OnStop(bool _) => simulationRunning = false;

    private void LateUpdate()
    {
        if (!simulationRunning || shipTransform == null) return;

        Vector3 pos = new Vector3(shipTransform.position.x, heightAboveWater, shipTransform.position.z);

        if (positions.Count == 0 || Vector3.Distance(pos, positions[^1]) >= recordInterval)
        {
            positions.Add(pos);
            Rebuild();
        }
    }

    private void Rebuild()
    {
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        lineRenderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 1e6f);
    }

    private void SetVisible(bool visible) => lineRenderer.enabled = visible;

    private void OnRestart(bool _)
    {
        positions.Clear();
        lineRenderer.positionCount = 0;
    }

    private void OnWorldRecentered(Vector3 offset)
    {
        for (int i = 0; i < positions.Count; i++)
            positions[i] += offset;
        if (positions.Count > 0)
            Rebuild();
    }
}
