using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryVisualizer : MonoBehaviour
{
	[Header("Ustawienia Trajektorii")]
	[Tooltip("Minimalny dystans w metrach, jaki musi pokonać statek, aby postawić nowy punkt")]
	public float minDistance = 1.0f;

	[Tooltip("Szerokość rysowanej linii kilwateru")]
	public float lineWidth = 2.0f;

	[Tooltip("Kolor rysowanej trajektorii")]
	public Color lineColor = Color.red;

	private LineRenderer lineRenderer;
	private Vector3 lastPoint;

	private void Awake()
	{
		// Pobieramy komponent LineRenderer (zostanie dodany automatycznie dzięki RequireComponent)
		lineRenderer = GetComponent<LineRenderer>();

		// Wstępna konfiguracja wyglądu linii
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		// Używamy prostego, wbudowanego shadera, który dobrze radzi sobie z jednolitymi kolorami
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = lineColor;
		lineRenderer.endColor = lineColor;

		// Linia ma być rysowana w przestrzeni świata, a nie lokalnie względem statku
		lineRenderer.useWorldSpace = true;
		lineRenderer.positionCount = 0;
	}

	private void Start()
	{
		// Dodajemy pierwszy punkt w miejscu startu
		AddPoint(transform.position);
	}

	private void FixedUpdate()
	{
		// Sprawdzamy dystans od ostatniego punktu - zapobiega to tworzeniu 
		// tysięcy punktów w tym samym miejscu, gdy statek stoi w miejscu
		if (Vector3.Distance(transform.position, lastPoint) >= minDistance)
		{
			AddPoint(transform.position);
		}
	}

	private void AddPoint(Vector3 position)
	{
		lineRenderer.positionCount++;

		// Dodajemy niewielki offset na osi Y (np. 0.1f), aby linia rysowała się delikatnie 
		// nad siatką, co zapobiega migotaniu tekstur (tzw. Z-fighting)
		Vector3 drawPos = new Vector3(position.x, position.y + 0.5f, position.z);

		lineRenderer.SetPosition(lineRenderer.positionCount - 1, drawPos);
		lastPoint = position;
	}

	// Pozwala na wyczyszczenie linii w dowolnym momencie z poziomu edytora
	[ContextMenu("Wyczyść Trajektorię")]
	public void ClearTrajectory()
	{
		lineRenderer.positionCount = 0;
		if (Application.isPlaying)
		{
			AddPoint(transform.position);
		}
	}
}