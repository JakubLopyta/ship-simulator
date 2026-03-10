using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
	[Header("Parametry siatki")]
	[Tooltip("Ilość linii w jednej osi (szerokość siatki)")]
	public int gridSize = 100;

	[Tooltip("Rozmiar pojedynczej komórki siatki w metrach")]
	public float cellSize = 10f;

	[Tooltip("Kolor rysowanej siatki")]
	public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

	[Header("Śledzenie")]
	[Tooltip("Jeśli true, siatka będzie podążać za obiektem, do którego jest podpięta")]
	public bool followTransform = true;

	private void OnDrawGizmos()
	{
		Gizmos.color = gridColor;

		// Ustalenie środka siatki
		Vector3 center = followTransform ? transform.position : Vector3.zero;

		// Jeśli siatka ma podążać za statkiem, "przyciągamy" jej środek do najbliższego przecięcia linii.
		// Dzięki temu siatka się nie przesuwa płynnie (nie "pływa"), tylko przeskakuje, dając złudzenie nieskończoności.
		if (followTransform)
		{
			center.x = Mathf.Round(center.x / cellSize) * cellSize;
			center.z = Mathf.Round(center.z / cellSize) * cellSize;
			center.y = 0f; // Zakładamy, że tafla wody to y = 0
		}

		float halfSize = (gridSize * cellSize) / 2f;

		// Rysowanie linii siatki
		for (int i = 0; i <= gridSize; i++)
		{
			float offset = -halfSize + (i * cellSize);

			// Linie równoległe do osi X
			Vector3 startX = new Vector3(center.x - halfSize, center.y, center.z + offset);
			Vector3 endX = new Vector3(center.x + halfSize, center.y, center.z + offset);
			Gizmos.DrawLine(startX, endX);

			// Linie równoległe do osi Z
			Vector3 startZ = new Vector3(center.x + offset, center.y, center.z - halfSize);
			Vector3 endZ = new Vector3(center.x + offset, center.y, center.z + halfSize);
			Gizmos.DrawLine(startZ, endZ);
		}

		// Opcjonalnie: Zaznaczenie głównych osi (X i Z) grubszym/jaśniejszym kolorem, jeśli jesteśmy blisko środka świata (0,0,0)
		if (!followTransform || (Mathf.Abs(center.x) < halfSize && Mathf.Abs(center.z) < halfSize))
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
			Gizmos.DrawLine(new Vector3(-halfSize, center.y, 0), new Vector3(halfSize, center.y, 0)); // Oś X
			Gizmos.DrawLine(new Vector3(0, center.y, -halfSize), new Vector3(0, center.y, halfSize)); // Oś Z
		}
	}
}