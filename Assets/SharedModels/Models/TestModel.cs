using Models.Models;
using System;
using UnityEngine;

public class TestModel : MonoBehaviour, IModel
{
	private Ship Ship;

	[Header("Ship parameters")]
	[Tooltip("Target top speed at full power, straight rudder. Expressed in m/s")]
	[SerializeField] private double vmax;
	[Tooltip("Infuluences only surge dynamics. Expressed in kg")]
	[SerializeField] private double mass;
	[Tooltip("Expressed in m")]
	[SerializeField] private double length;

	[Header("State")]
	[Tooltip("(-pi:pi) 0 -> North, +pi/2 -> East")]
	[SerializeField] private double HeadingRad = 0;
	[SerializeField] private double YawRateRad = 0;
	[SerializeField] private double currentRudderDeg;

	[Header("Surge tuning")]
	[Tooltip("N at enginePower = 1")]
	[SerializeField] private double thrustForce = 5e5;
	[Tooltip("Extra drag multiplier for rudder angle^2 (1.0-3.0)")]
	[SerializeField] private double rudderDragK = 1.8;    
	
	[Header("Yaw tuning")]
	[Tooltip("How strongly rudder turns the ship")]
	[SerializeField] private double yawGain = 0.5;
	[Tooltip("How fast yawRate reaches target rate in rad/s^2")]
	[SerializeField] private double YawAccel = 5e-3;

	[Space(10)]
	[Tooltip("Maximum rudder deviation in deg")]
	[SerializeField] private double rudderMax = 35;

	// User inputs
	private float enginePower = 0f;
	private float rudderTarget = 0f;

	void Awake()
	{
		Ship = GetComponent<Ship>();
	}
	void OnEnable()
	{
		length = Ship.Length;
		vmax = Ship.Vmax;
		mass = Math.Max(1, Ship.Displacement * 1000);  // scale from t to kg
		thrustForce = 5e5;
		rudderMax = Ship.RudderMax;
		rudderDragK = 1.8;
	}
	private void FixedUpdate()
	{
		Calculate();
	}
	public void Calculate()
	{
		float dt = Time.deltaTime;
		enginePower = Ship.EnginePower;
		rudderTarget = Ship.RudderTarget;
		double v = Ship.Speed;

		#region Yaw / turning dynamics 
		// Simple empirical yaw model: desired yaw rate is proportional to rudder * (speed / length)
		// Limit rudder deflection rate
		currentRudderDeg = Mathf.MoveTowardsAngle((float)currentRudderDeg, rudderTarget, 2.6f * dt);

		double rudderNorm = currentRudderDeg / rudderMax; // -1..1
		double yawTarget = yawGain * rudderNorm * (v / Math.Max(length, 10)); // rad/s
		YawRateRad = Mathf.MoveTowards((float)YawRateRad, (float)yawTarget, (float)(YawAccel * dt));
		HeadingRad += YawRateRad * dt;		// update current heading

		// Wrap heading to [-pi, pi] for stability
		if (HeadingRad > Math.PI)
			HeadingRad -= 2 * Math.PI;
		else if (HeadingRad < -Math.PI)
			HeadingRad += 2 * Math.PI;
		#endregion

		#region Acceleration - thrust and drag
		// Calculate drag so that: thrust ~= drag at vmax with straight rudder (vessel is not accelerating)
		// dragForce0 (rudder=0) = thrustForce (at vmax)
		// dragForce0 (rudder=0) = dragCoefficient0 * speed^2
		// dragCoefficient0 * speed^2 = thrustForce  -->  dragCoefficient0 = thrustForce / speed^2
		double dragCoefficient0 = (vmax != 0) ? (thrustForce / (vmax * vmax)) : 0;

		// Rudder-induced drag
		double rudderFrac = Math.Abs(rudderNorm);
		double dragCoefficient = dragCoefficient0 * (1 + rudderDragK * rudderFrac * rudderFrac);

		// Speed drag
		double dragForce = dragCoefficient * v * v * Math.Sign(v);  // frictional resistance proportional to v^2

		double actualThrustForce = thrustForce * enginePower;
		double netForce = actualThrustForce - dragForce;    // N
		double accel = netForce / mass;
		v += accel * dt;

		// Prevent tiny float tails and negative creep when power is zero
		if (enginePower <= 0.0001 && Math.Abs(v) < 0.01)
			v = 0f;
		v = Math.Max(0, v);

		Ship.Speed = v;
		#endregion

		#region Spherical Earth position update
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
		#endregion

		#region Ship state update
		// Cog and Hdg are temporarily the same
		Ship.Cog = HeadingRad * 180 / Math.PI;
		Ship.Hdg = Ship.Cog;
		Ship.Rot = YawRateRad * 180 / Math.PI;
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
		#endregion
	}

	[ContextMenu("Reset state")]
	public void ResetState()
	{
		YawRateRad = 0;
		Ship.Rot = YawRateRad;
		Ship.Sog = Ship.Speed;
	}
}
