using UnityEngine;

public class TrajectoryVisualizer : MonoBehaviour
{
	[Tooltip("Minimum distance in meters ship has to travesrse to place new point")]
	public float minDistance = 1.0f;
	public float lineWidth = 2.0f;
	public Color lineColor = Color.red;

	private LineRenderer lineRenderer;
	private Vector3 lastPoint;
	private GameObject lineObject;

	private void Awake()
	{
		lineObject = new GameObject("TrajectoryLine_" + gameObject.name);
		lineObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

		lineRenderer = lineObject.AddComponent<LineRenderer>();
		lineRenderer.alignment = LineAlignment.TransformZ;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
		lineRenderer.material.color = lineColor;
		
		lineRenderer.useWorldSpace = true;
		lineRenderer.positionCount = 0;
	}

	private void Start()
	{
		AddPoint(transform.position);
	}

	private void FixedUpdate()
	{
		if (Vector3.Distance(transform.position, lastPoint) >= minDistance)
		{
			AddPoint(transform.position);
		}
	}

	private void AddPoint(Vector3 position)
	{
		lineRenderer.positionCount++;

		Vector3 drawPos = new Vector3(position.x, position.y + 0.2f, position.z);

		lineRenderer.SetPosition(lineRenderer.positionCount - 1, drawPos);
		lastPoint = position;
	}

	[ContextMenu("Clear trajectory")]
	public void ClearTrajectory()
	{
		lineRenderer.positionCount = 0;
		if (Application.isPlaying)
		{
			AddPoint(transform.position);
		}
	}

	private void OnDestroy()
	{
		if (lineObject != null)
		{
			Destroy(lineObject);
		}
	}
}