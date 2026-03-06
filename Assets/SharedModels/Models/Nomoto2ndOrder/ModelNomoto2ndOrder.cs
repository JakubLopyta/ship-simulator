using Models.Models;
using UnityEngine;

/// <summary>
/// Implementation of 2nd order Nomoto model with surge dynamics for ship movement testing
/// </summary>
public class Nomoto2ndOrderModel: MonoBehaviour, IModel
{
	public Nomoto2ndOrderModel(Ship _ship)
	{
		Ship = _ship;
	}

	[SerializeField] private Ship Ship;

	[Header("Nomoto parameters (dimensionless)")]
	[SerializeField] private float T1 = 15f;
	[SerializeField] private float T2 = 2f;
	[SerializeField] private float T3 = 4f;
	[SerializeField] private float K = 0.05f;

	// Kinematic state variables
	private float YawRate = 0f;			// r [rad/s]
	private float YawAccel = 0f;		// r_dot [rad/s^2]
	private float HeadingRad = 0f;      // psi [rad]

	// User inputs
	private float enginePower = 0f;
	private float rudderDeg = 0f;

	// Memory of previous rudder to calculate derivative
	private float currentRudderAngle;
	private float previousRudderAngle = 0f;
	private bool isFirstUpdate = true;

	// temp for surge calculations
	[SerializeField] private float vmax;            // m/s
	[SerializeField] private uint mass;				// kg
	[SerializeField] private float thrustForce;     // N at enginePower=1
	[SerializeField] private float rudderMax;		// deg
	[SerializeField] private float rudderDragK;		// extra drag multiplier for rudder angle^2 (1.0-3.0)

	void Awake()
	{
		Ship = GetComponent<Ship>();
	}
	void OnEnable()
	{
		vmax = Ship.Vmax;
		mass = Ship.Displacement;
		thrustForce = 5e5f;
		rudderMax = Ship.RudderMax;
		rudderDragK = 1.8f;
	}
	void FixedUpdate()
	{
		Calculate();
	}

	/// <summary>
	/// Method updating ship state
	/// </summary>
	public void Calculate()
	{
		float dt = Time.fixedDeltaTime;
		rudderDeg = Mathf.Clamp((float)Ship.Rudder, -35f, 35f);
		currentRudderAngle = rudderDeg * Mathf.Deg2Rad;
		mass = (uint)Mathf.Clamp(Ship.Displacement, 1000, 1000000000);   // minimum one tonne, maximum milion tonns
		enginePower = Ship.EnginePower;
		float v = (float)Ship.Speed;
		float l = Ship.Length;
		

		#region Nomoto2ndOrder
		// Switch to dimensional parameters
		float safeV = Mathf.Max(v, 0.5f);
		float K_dim = K * safeV / l;
		float lov = l / safeV;
		float T1_dim = T1 * lov;
		float T2_dim = T2 * lov;
		float T3_dim = T3 * lov;

		// Obliczenie pochodnej wychylenia steru (delta_dot)
		float delta_dot = 0f;
		if (isFirstUpdate)
			isFirstUpdate = false;
		else
			delta_dot = (currentRudderAngle - previousRudderAngle) / dt;

		// Przygotowanie mianowników
		float T1T2 = T1_dim * T2_dim;
		float T1plusT2 = T1_dim + T2_dim;

		// Obliczenie drugiej pochodnej prędkości kątowej (r_dot_dot)
		// r_dot_dot = [ K*(delta + T3*delta_dot) - (T1+T2)*r_dot - r ] / (T1*T2)
		float term1 = K_dim * (currentRudderAngle + T3_dim * delta_dot);
		float term2 = T1plusT2 * YawAccel;
		float term3 = YawRate;

		float r_dot_dot = (term1 - term2 - term3) / T1T2;

		// Semi implicit Euler (Euler-Cromer) - sequential variable updating
		YawAccel += r_dot_dot * dt;
		YawRate += YawAccel * dt;
		HeadingRad += YawRate * dt;

		// Zapamiętanie kąta steru do obliczenia delta_dot w następnym kroku
		previousRudderAngle = currentRudderAngle;
		#endregion


		#region Acceleration - thrust and drag
		// Calculate drag so that: thrust ~= drag at vmax with straight rudder (vessel is not accelerating)
		// dragForce0 (rudder=0) = thrustForce (at vmax)
		// dragForce0 (rudder=0) = dragCoefficient0 * speed^2
		// dragCoefficient0 * speed^2 = thrustForce  -->  dragCoefficient0 = thrustForce / speed^2
		float dragCoefficient0 = (vmax != 0f) ? (thrustForce / (vmax * vmax)) : 0f;

		// Rudder-induced drag
		float rudderFrac = Mathf.Abs(rudderDeg) / rudderMax;
		float dragCoefficient = dragCoefficient0 * (1f + rudderDragK * rudderFrac * rudderFrac);

		// Speed drag
		float dragForce = dragCoefficient * v * v * Mathf.Sign(v);  // frictional resistance proportional to v^2

		float actualThrustForce = thrustForce * enginePower;
		float netForce = actualThrustForce - dragForce;    // N
		float accel = netForce / mass;
		v += accel * dt;

		// Prevent tiny float tails and negative creep when power is zero
		if (enginePower <= 0.0001f && Mathf.Abs(v) < 0.01f)
			v = 0f;
		v = Mathf.Max(0f, v);

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
		float vNorth = v * Mathf.Cos(HeadingRad);  // 0 rad = North
		float vEast = v * Mathf.Sin(HeadingRad);  // +pi/2 rad = East

		xEast += vEast * dt;
		yNorth += vNorth * dt;

		// --------- Update ship state ----------
		// Cog and Hdg are temporarily the same
		Ship.Cog = HeadingRad * Mathf.Rad2Deg;
		Ship.Hdg = Ship.Cog;
		Ship.Rot = YawRate * Mathf.Rad2Deg;
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
		Ship.Speed = 0;
		YawRate = 0f;
		YawAccel = 0f;
		HeadingRad = 0f;
		previousRudderAngle = 0f;
		isFirstUpdate = true;
	}
}