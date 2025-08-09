using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;       // Obiekt, wokół którego kamera się kręci
    public float distance = 100f;   // Początkowy dystans
    public float zoomSpeed = 100f;   // Szybkość zoomu
    public float minDistance = 250f;
    public float maxDistance = 1000f;

    public float xSpeed = 120f;    // Szybkość obrotu w poziomie
    public float ySpeed = 80f;     // Szybkość obrotu w pionie

    public float yMinLimit = -20f; // Minimalny kąt patrzenia w dół
    public float yMaxLimit = 80f;  // Maksymalny kąt patrzenia w górę

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

        // Ukryj kursor, jeśli chcesz
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Obracanie tylko jeśli trzymasz lewy przycisk myszy
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

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