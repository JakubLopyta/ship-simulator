using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
	[Header("Grid Parameters")]
	[Tooltip("Number of lines on one axis (grid width)")]
	public int gridSize = 100;

	[Tooltip("Size of a single grid cell in meters")]
	public float cellSize = 10f;

	[Tooltip("Color of the drawn grid")]
	public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

	[Header("Tracking")]
	[Tooltip("If true, the grid will follow the object it is attached to")]
	public bool followTransform = true;

	private void OnDrawGizmos()
	{
		Gizmos.color = gridColor;

		// Determine the grid center
		Vector3 center = followTransform ? transform.position : Vector3.zero;

		// If the grid should follow the ship, snap its center to the nearest grid intersection.
		// This prevents the grid from scrolling smoothly (no "floating") — it snaps instead, creating the illusion of an infinite grid.
		if (followTransform)
		{
			center.x = Mathf.Round(center.x / cellSize) * cellSize;
			center.z = Mathf.Round(center.z / cellSize) * cellSize;
			center.y = 0f; // Assume the water surface is at y = 0
		}

		float halfSize = (gridSize * cellSize) / 2f;

		// Draw grid lines
		for (int i = 0; i <= gridSize; i++)
		{
			float offset = -halfSize + (i * cellSize);

			// Lines parallel to the X axis
			Vector3 startX = new Vector3(center.x - halfSize, center.y, center.z + offset);
			Vector3 endX = new Vector3(center.x + halfSize, center.y, center.z + offset);
			Gizmos.DrawLine(startX, endX);

			// Lines parallel to the Z axis
			Vector3 startZ = new Vector3(center.x + offset, center.y, center.z - halfSize);
			Vector3 endZ = new Vector3(center.x + offset, center.y, center.z + halfSize);
			Gizmos.DrawLine(startZ, endZ);
		}

		// Optional: highlight the main axes (X and Z) with a brighter color when near the world origin (0,0,0)
		if (!followTransform || (Mathf.Abs(center.x) < halfSize && Mathf.Abs(center.z) < halfSize))
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
			Gizmos.DrawLine(new Vector3(-halfSize, center.y, 0), new Vector3(halfSize, center.y, 0)); // X axis
			Gizmos.DrawLine(new Vector3(0, center.y, -halfSize), new Vector3(0, center.y, halfSize)); // Z axis
		}
	}
}