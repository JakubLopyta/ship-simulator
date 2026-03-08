using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;       // Obiekt, wokół którego kamera się kręci
    [SerializeField] private float distance = 100f;   // Początkowy dystans
    [SerializeField] private float zoomSpeed = 100f;   // Szybkość zoomu
    [SerializeField] private float minDistance = 250f;
    [SerializeField] private float maxDistance = 1000f;

    [SerializeField] private float xSpeed = 120f;    // Szybkość obrotu w poziomie
    [SerializeField] private float ySpeed = 80f;     // Szybkość obrotu w pionie

    [SerializeField] private float yMinLimit = -20f; // Minimalny kąt patrzenia w dół
    [SerializeField] private float yMaxLimit = 80f;  // Maksymalny kąt patrzenia w górę

    private float x = 0f;
    private float y = 0f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            // Opcjonalnie ustaw początkowe kąty obrotu na aktualne
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            // Ustaw też dystans domyślny (opcjonalne)
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Obracanie tylko jeśli trzymasz prawy przycisk myszy
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.unscaledDeltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.unscaledDeltaTime;

            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        }

        // Zoom – scroll mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        // Oblicz nową pozycję kamery
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
