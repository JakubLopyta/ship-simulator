using Models.Models;
using System;
using UnityEngine;

/// <summary>
/// Implementation of 2nd order Nomoto model with surge dynamics for ship movement testing
/// </summary>
public class ModelNomoto2ndOrder: MonoBehaviour, IModel
{
	private Ship Ship;

	[Header("Nomoto parameters (dimensionless)")]
	[SerializeField] private float K;
	[SerializeField] private float T1;
	[SerializeField] private float T2;
	[SerializeField] private float T3;

	[Header("Ship parameters")]
	[Tooltip("Target top speed at full power, straight rudder. Expressed in m/s")]
	[SerializeField] private double vmax;
	[Tooltip("Infuluences only surge dynamics. Expressed in kg")]
	[SerializeField] private double mass;
	[Tooltip("Expressed in m")]
	[SerializeField] private double length;

	[Header("State")]
	[Tooltip("Expressed in rad/s")]
	[SerializeField] private double YawRate = 0;		// r
	[Tooltip("Expressed in rad/s^2")]
	[SerializeField] private double YawAccel = 0;		// r_dot
	[SerializeField] private double HeadingRad = 0;	// psi

	[SerializeField] private double currentRudderDeg;
	[SerializeField] private double currentRudderRad;
	private double previousRudderAngle = 0; // Memory of previous rudder to calculate derivative
	private bool isFirstUpdate = true;

	// Surge tuning
	[Header("Surge tuning")]
	[Tooltip("N at enginePower = 1")]
	[SerializeField] private double thrustForce;
	[Tooltip("Maximum rudder deviation in deg")]
	[SerializeField] private double rudderMax;
	[Tooltip("Extra drag multiplier for rudder angle^2 (1.0-3.0)")]
	[SerializeField] private double rudderDragK;

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
	/// <summary>
	/// Method updating ship state
	/// </summary>
	public void Calculate()
	{
		float dt = Time.fixedDeltaTime;
		rudderTarget = Ship.RudderTarget;
		enginePower = Ship.EnginePower;
		double v = Ship.Speed;		

		#region Nomoto2ndOrder
		// Switch to dimensional parameters
		double safeV = Math.Max(v, 0.5);
		double K_dim = K * safeV / length;
		double lov = length / safeV;
		double T1_dim = T1 * lov;
		double T2_dim = T2 * lov;
		double T3_dim = T3 * lov;

		// Limit rudder deflection rate
		currentRudderDeg = Mathf.MoveTowardsAngle((float)currentRudderDeg, rudderTarget, 2.6f * dt);
		currentRudderRad = currentRudderDeg * Math.PI / 180;

		// Rudder deflection derivative calculation (delta_dot)
		double delta_dot = 0;
		if (isFirstUpdate)
			isFirstUpdate = false;
		else
			delta_dot = (currentRudderRad - previousRudderAngle) / dt;

		// Preparing denominators
		double T1T2 = T1_dim * T2_dim;
		double T1plusT2 = T1_dim + T2_dim;

		// r_dot_dot = [ K*(delta + T3*delta_dot) - (T1+T2)*r_dot - r ] / (T1*T2)
		double term1 = K_dim * (currentRudderRad + T3_dim * delta_dot);
		double term2 = T1plusT2 * YawAccel;
		double term3 = YawRate;

		double r_dot_dot = (term1 - term2 - term3) / T1T2;

		// Semi implicit Euler (Euler-Cromer) - sequential variable updating
		YawAccel += r_dot_dot * dt;
		YawRate += YawAccel * dt;
		HeadingRad += YawRate * dt;

		// Wrap heading to [-pi, pi] for stability
		if (HeadingRad > Math.PI)
			HeadingRad -= 2 * Math.PI;
		else if (HeadingRad < -Math.PI)
			HeadingRad += 2 * Math.PI;

		// Saving rudder angle to calculate delta_dot in next simulation step
		previousRudderAngle = currentRudderRad;
		#endregion

		#region Acceleration - thrust and drag
		// Calculate drag so that: thrust ~= drag at vmax with straight rudder (vessel is not accelerating)
		// dragForce0 (rudder=0) = thrustForce (at vmax)
		// dragForce0 (rudder=0) = dragCoefficient0 * speed^2
		// dragCoefficient0 * speed^2 = thrustForce  -->  dragCoefficient0 = thrustForce / speed^2
		double dragCoefficient0 = (vmax != 0) ? (thrustForce / (vmax * vmax)) : 0;

		// Rudder-induced drag
		double rudderFrac = Math.Abs(currentRudderDeg) / rudderMax;
		double dragCoefficient = dragCoefficient0 * (1 + rudderDragK * rudderFrac * rudderFrac);

		// Speed drag
		double dragForce = dragCoefficient * v * v * Math.Sign(v);  // frictional resistance proportional to v^2

		double actualThrustForce = thrustForce * enginePower;
		double netForce = actualThrustForce - dragForce;    // N
		double accel = netForce / mass;
		v += accel * dt;

		// Prevent tiny tails and negative creep when power is zero
		if (enginePower <= 0.0001 && Math.Abs(v) < 0.01)
			v = 0f;
		v = Math.Max(0, v);

		Ship.Speed = v;
		#endregion

		#region Spherical Earth position update
		OriginManager manager = OriginManager.Instance;
		if (manager == null) Debug.LogError("NomotoModel: OriginManager not found!");

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
		#endregion
	}

	public void SetNomotoParameters(float K, float T1, float T2, float T3)
	{
		this.K = K;
		this.T1 = T1;
		this.T2 = T2;
		this.T3 = T3;
	}

	[ContextMenu("Reset state")]
	public void ResetState()
	{
		YawRate = 0;
		YawAccel = 0;
		currentRudderDeg = 0;
		previousRudderAngle = 0;
		isFirstUpdate = true;
		Ship.Sog = Ship.Speed;
		Ship.Rot = YawRate;
	}
}