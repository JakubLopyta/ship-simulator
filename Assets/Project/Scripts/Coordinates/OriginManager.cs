using UnityEngine;
using System;

/// <summary>
/// Manages a "floating" ENU (East-North-Up) origin to maintain
/// floating-point precision in a large-scale world.
/// The origin is centered on the playerShip.
/// </summary>
public class OriginManager : MonoBehaviour
{
    public static event Action<Vector3> OnWorldRecentered;
	public static OriginManager Instance { get; private set; }
	private Ship playerShip;
	[SerializeField] private float recenterThreshold = 1000f;

	public double OriginLatitude { get; private set; }
	public double OriginLongitude { get; private set; }
	public double OriginHeight { get; private set; }
	
	private bool isInitialized = false;

	void Awake()
	{
		// Setup Singleton
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	void LateUpdate()
	{
		if (!isInitialized || playerShip == null) return;

		// Check if the player has moved beyond the threshold
		if (playerShip.transform.position.sqrMagnitude > recenterThreshold * recenterThreshold)
			RecenterOrigin();
	}

	public void Initialize(Ship player)
	{
		if (player == null)
		{
			Debug.LogError("EnuOriginManager: Initialize called with null player ship!");
			return;
		}

		playerShip = player;
		// Set the initial origin to the player's starting position
		SetOrigin(playerShip.LatitudeDeg, playerShip.LongitudeDeg, playerShip.Height);
		isInitialized = true;
		Debug.Log("EnuOriginManager initialized and origin set.");

	}

	/// <summary>
	/// Recenters the world origin to the local player's current position
	/// and repositions all other ships relative to this new origin.
	/// </summary>
	private void RecenterOrigin()
	{
		Ship localShip = playerShip.GetComponent<Ship>();
		if (localShip == null) return;

		Vector3 worldOffset = -playerShip.transform.position;

		// 1. Set the new origin to the player's current true (Geodetic) position
		SetOrigin(localShip.LatitudeDeg, localShip.LongitudeDeg, localShip.Height);

		// 2. Reposition all ships relative to the new origin
		Ship[] allShips = FindObjectsByType<Ship>(FindObjectsSortMode.None);
		foreach (Ship ship in allShips)
		{
			// Convert this ship's ECEF position to an ENU relative to the NEW origin.
			CoordinatesConversion.EcefToEnu(ship.EcefX, ship.EcefY, ship.EcefZ,
										OriginLatitude, OriginLongitude, OriginHeight,
										out double xEast, out double yNorth, out double zUp);
			// Set the ship's transform position in Unity space
			ship.transform.position = new Vector3((float)xEast, (float)zUp, (float)yNorth);
		}

		OnWorldRecentered?.Invoke(worldOffset);

		Debug.Log($"Recentered origin to: {OriginLatitude}; {OriginLongitude}\n" +
			$"Updated position of {allShips.Length} ships");
	}

	/// <summary>
	/// Sets the ENU origin to a new Geodetic coordinate.
	/// </summary>
	public void SetOrigin(double lat, double lon, double h)
	{
		OriginLatitude = lat;
		OriginLongitude = lon;
		OriginHeight = h;
	}
}