// A Unity module that converts geographic coordinates (latitude, longitude in degrees)
// to flat world positions (X, Z) using a WGS84 ellipsoid-based Web Mercator projection (EPSG:3857).
//
// Compared to spherical Mercator, this uses the semi-major axis of WGS84 (a = 6378137 m)
// and the eccentricity e = sqrt(f * (2 - f)), where f = 1/298.257223563.
// This is the same projection as used in most web mapping systems (Google, OSM).
//
// Usage:
// 1) Attach MercatorMapperWGS84 to a GameObject.
// 2) Set the geographic center in inspector.
// 3) Use LatLonToWorldPosition to convert coordinates.


// WORKS CORRECTLY ONLY TO +-85 DEGREES OF LATITUDE

using UnityEngine;

[ExecuteInEditMode]
public class MercatorMapperWGS84 : MonoBehaviour
{
    [Header("Map origin (geographic)")]
    public double centerLatitude = 0.0;
    public double centerLongitude = 0.0;

    [Header("Scaling")]
    [Tooltip("How many meters correspond to 1 Unity unit. 1 = 1 meter.")]
    public float metersPerUnit = 1f;

    [Header("Vertical offset")]
    [Tooltip("Y coordinate for placed objects (typical terrain height).")]
    public float worldY = 0f;

	private Vector2 centerMeters;

    void OnValidate()
    {
        centerMeters = MercatorProjectionWGS84.LatLonToMeters(centerLatitude, centerLongitude);
    }

    void Awake()
    {
        centerMeters = MercatorProjectionWGS84.LatLonToMeters(centerLatitude, centerLongitude);
    }

    public Vector3 LatLonToWorldPosition(double latitude, double longitude)
    {
        Vector2 meters = MercatorProjectionWGS84.LatLonToMeters(latitude, longitude);
        double localX = meters.x - centerMeters.x;
        double localY = meters.y - centerMeters.y;

        float ux = (float)(localX / metersPerUnit);
        float uz = (float)(localY / metersPerUnit);

        return new Vector3(ux, worldY, uz);
    }

    public (double lat, double lon) WorldPositionToLatLon(Vector3 worldPos)
    {
        double metersX = centerMeters.x + (worldPos.x * metersPerUnit);
        double metersY = centerMeters.y + (worldPos.z * metersPerUnit);
        var latlon = MercatorProjectionWGS84.MetersToLatLon(metersX, metersY);
        return latlon;
    }

    public GameObject SpawnPrefabAt(GameObject prefab, double latitude, double longitude)
    {
        if (prefab == null) return null;
        Vector3 pos = LatLonToWorldPosition(latitude, longitude);
        GameObject go = Instantiate(prefab, pos, Quaternion.identity, this.transform);
        return go;
    }
}
