using UnityEngine;

public class MapEntity : MonoBehaviour
{
    [Tooltip("Przeci¹gnij tutaj plik ScriptableObject z konfiguracj¹ ikony (np. ShipIconData)")]
    public MapIconData IconConfig;

    [Tooltip("Nadpisz promieñ. Jeœli 0, u¿yje promienia z konfiguracji.")]
    public float CustomRadius = 0f;

    private bool isRegistered = false;

    public float GetRadius() => CustomRadius > 0f ? CustomRadius : (IconConfig != null ? IconConfig.DefaultRadius : 10f);

    private void OnEnable()
    {
        if (!isRegistered && MapEntityManager.Instance != null)
        {
            MapEntityManager.Instance.Register(this);
            isRegistered = true;
        }
    }

    private void OnDisable()
    {
        if (MapEntityManager.Instance != null)
        {
            MapEntityManager.Instance.Unregister(this);
        }
    }
}