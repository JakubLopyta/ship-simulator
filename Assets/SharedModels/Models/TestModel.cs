using Models.Models;
using System;
using UnityEngine;

public class TestModel : MonoBehaviour, IModel
{
	// Ship parameters
	public Ship Ship;
	public float vmax;              // m/s target top speed at full power, straight rudder
	public float length;
	public float mass;	// kg

	// State
	public double HeadingRad = 0f;    // 0=N, +pi/2=E
	public double YawRate = 0f;       // rad/s

	// User inputs
	[Range(0, 1)] public float enginePower = 0f;
	[Range(-35, 35)] public float rudderDeg = 0f;

	// Tunning
	public float thrustForce = 5e5f;    // N at enginePower=1
	public float rudderMax = 35f;		// deg
	public float rudderDragK = 1.8f;    // extra drag multiplier for rudder angle^2 (1.0-3.0)
	public float yawGain = 0.5f;        // how strongly rudder turns the ship
	public float YawAccel = 5e-3f;      // rad/s^2, how fast yawRate reaches target rate

	void Awake()
	{
		Ship = GetComponent<Ship>();
	}
	void OnEnable()
	{
		vmax = Ship.Vmax;
		mass = Mathf.Max(1, Ship.Displacement * 1000);  // scale from t to kg
		thrustForce = 5e5f;
		rudderMax = Ship.RudderMax;
		rudderDragK = 1.8f;
	}
	private void FixedUpdate()
	{
		Calculate();
	}
	public void Calculate()
	{
		float dt = Time.deltaTime;
		length = Ship.Length;
		enginePower = Ship.EnginePower;
		rudderDeg = Ship.RudderTarget;
		

		// ---------- Acceleration - thrust and drag ----------
		// Calculate drag so that: thrust ~= drag at vmax with straight rudder (vessel is not accelerating)
		// dragForce0 (rudder=0) = thrustForce (at vmax)
		// dragForce0 (rudder=0) = dragCoefficient0 * speed^2
		// dragCoefficient0 * speed^2 = thrustForce  -->  dragCoefficient0 = thrustForce / speed^2
		float dragCoefficient0 = (vmax != 0f) ? (thrustForce / (vmax * vmax)) : 0f;

		// Rudder-induced drag
		double rudderFrac = Mathf.Abs(rudderDeg) / rudderMax;
		double dragCoefficient = dragCoefficient0 * (1 + rudderDragK * rudderFrac * rudderFrac);

		// Speed drag
		double v = Ship.Speed;
		double dragForce = dragCoefficient * v * v * Math.Sign(v);	// frictional resistance proportional to v^2

		float actualThrustForce = thrustForce * enginePower;
		double netForce = actualThrustForce - dragForce;    // N
		double accel = netForce / mass;
		v += accel * dt;

		// Prevent tiny float tails and negative creep when power is zero
		if (enginePower <= 0.0001 && Math.Abs(v) < 0.01)
			v = 0f;
		v = Math.Max(0, v);

		Ship.Speed = v;


		// ---------- Yaw / turning dynamics ----------
		// Simple empirical yaw model: desired yaw rate is proportional to rudder * (speed / length)
		float rudderNorm = rudderDeg / rudderMax; // -1..1
		float yawTarget = yawGain * rudderNorm * ((float)v / Mathf.Max(length, 10f)); // rad/s
		YawRate = Mathf.MoveTowards((float)YawRate, yawTarget, YawAccel * dt);
		HeadingRad += YawRate * dt;		// update current heading

		// Wrap heading to [-pi, pi] for stability
		if (HeadingRad > Math.PI)
			HeadingRad -= 2 * Math.PI;
		else if (HeadingRad < -Math.PI)
			HeadingRad += 2 * Math.PI;


		// ---------- Spherical Earth position update ----------
		OriginManager manager = OriginManager.Instance;
		if(manager == null) return;

		double xEast, yNorth, zUp;
		CoordinatesConversion.EcefToEnu(
			Ship.EcefX,
			Ship.EcefY,
			Ship.EcefZ,
			manager.OriginLatitude,
			manager.OriginLongitude,
			manager.OriginHeight,
			out xEast,
			out yNorth,
			out zUp);
				
		// Decompose ground speed into North/East components in meters/second.
		double vNorth = v * Math.Cos(HeadingRad);  // 0 rad = North
		double vEast = v * Math.Sin(HeadingRad);  // +pi/2 rad = East
		
		xEast += vEast * dt;
		yNorth += vNorth * dt;
		

		// --------- Update ship state ----------
		// Cog and Hdg are temporarily the same
		Ship.Cog = HeadingRad * 180 / Math.PI;
		Ship.Hdg = Ship.Cog;
		Ship.Rot = YawRate * 180 / Math.PI;
		Ship.Sog = Ship.Speed;

		Vector3 unityPos = new Vector3((float)xEast, (float)zUp, (float)yNorth);
		Quaternion unityRotation = Quaternion.Euler(0f, (float)Ship.Cog, 0f);
		Ship.transform.SetPositionAndRotation(unityPos, unityRotation);
		CoordinatesConversion.EnuToEcef(
			xEast,
			yNorth,
			zUp,
			manager.OriginLatitude,
			manager.OriginLongitude,
			manager.OriginHeight,
			out Ship.EcefX,
			out Ship.EcefY,
			out Ship.EcefZ);
		CoordinatesConversion.EcefToGeodetic(
			Ship.EcefX,
			Ship.EcefY,
			Ship.EcefZ,
			out Ship.LatitudeDeg,
			out Ship.LongitudeDeg,
			out Ship.Height);
	}

	[ContextMenu("Reset state")]
	public void ResetState()
	{
		Ship.Speed = 0;
		YawRate = 0;
		HeadingRad = 0;
	}
}
