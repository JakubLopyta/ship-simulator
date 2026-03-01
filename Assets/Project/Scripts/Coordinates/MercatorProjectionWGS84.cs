// A Unity module that converts geographic coordinates (latitude, longitude in degrees)
// to flat world positions (X, Z) using a WGS84 ellipsoid-based Web Mercator projection (EPSG:3857).
//
// Compared to spherical Mercator, this uses the semi-major axis of WGS84 (a = 6378137 m)
// and the eccentricity e = sqrt(f * (2 - f)), where f = 1/298.257223563.
// This is the same projection as used in most web mapping systems (Google, OSM).


// WORKS CORRECTLY ONLY TO +-85 DEGREES OF LATITUDE

using UnityEngine;
using System;

public static class MercatorProjectionWGS84
{
    // WGS84 constants
    public const double a = 6378137.0;                 // semi-major axis (meters)
    public const double f = 1.0 / 298.257223563;       // flattening
    public const double e = 0.0818191908426215;        // eccentricity (precomputed: e^2 = 2f - f^2)

    // Convert latitude (deg) to Mercator Y (meters)
    public static double LatToMercatorY(double latDeg)
    {
        double latRad = latDeg * Mathf.Deg2Rad;
        // clamp to avoid singularities
        if (latRad > (Math.PI / 2.0 - 1e-10)) latRad = Math.PI / 2.0 - 1e-10;
        if (latRad < (-Math.PI / 2.0 + 1e-10)) latRad = -Math.PI / 2.0 + 1e-10;

        double sinLat = Math.Sin(latRad);
        double term = (1 - e * sinLat) / (1 + e * sinLat);
        double y = a * Math.Log(Math.Tan(Math.PI / 4.0 + latRad / 2.0) * Math.Pow(term, e / 2.0));
        return y;
    }

    // Convert longitude (deg) to Mercator X (meters)
    public static double LonToMercatorX(double lonDeg)
    {
        double lonRad = lonDeg * Mathf.Deg2Rad;
        return a * lonRad;
    }

    // Inverse: Mercator Y (meters) -> latitude (degrees)
    public static double MercatorYToLat(double y)
    {
        double t = Math.Exp(-y / a);
        double phi = Math.PI / 2 - 2 * Math.Atan(t);
        // Iterative refinement for ellipsoid
        for (int i = 0; i < 5; i++)
        {
            double sinPhi = Math.Sin(phi);
            double newPhi = Math.PI / 2 - 2 * Math.Atan(t * Math.Pow((1 - e * sinPhi) / (1 + e * sinPhi), e / 2.0));
            phi = newPhi;
        }
        return phi * Mathf.Rad2Deg;
    }

    // Inverse: Mercator X (meters) -> longitude (degrees)
    public static double MercatorXToLon(double x)
    {
        double lonRad = x / a;
        return lonRad * Mathf.Rad2Deg;
    }

    // Convenience: LatLon (deg) -> point in Mercator meters
    public static Vector2 LatLonToMeters(double latDeg, double lonDeg)
    {
        double x = LonToMercatorX(lonDeg);
        double y = LatToMercatorY(latDeg);
        return new Vector2((float)x, (float)y);
    }

    // Convenience: meters -> LatLon (deg)
    public static (double lat, double lon) MetersToLatLon(double x, double y)
    {
        double lat = MercatorYToLat(y);
        double lon = MercatorXToLon(x);
        return (lat, lon);
    }
}
