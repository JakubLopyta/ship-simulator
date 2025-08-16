using Models.Models;
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEditor.PlayerSettings;

public class TestModel : IModel, IDisposable
{
	public TestModel(double _hdgK, double _hdgsigmaC, double _hdgT, double _cogK, double _cogsigmaC, double _cogT,
			double _posX, double _posY, double _speed, float _vmax, double _inertia, double _length, double _width, double _rudlpersec, double _rudlmax, Ship _ship)
	{
		hdgK = _hdgK;
		hdgsigmaC = _hdgsigmaC;
		hdgT = _hdgT;
		cogK = _cogK;
		cogsigmaC = _cogsigmaC;
		cogT = _cogT;
		vmax = _vmax;
		inertia = _inertia;
		rudlpersec = _rudlpersec;
		ship = _ship;
		ship.Length = _length;
		ship.Width = _width;
		ship.PosX = _posX;
		ship.PosY = _posY;
		ship.Speed = _speed;
		ship.RudlMax = _rudlmax;
	}

	public Ship ship { get; set; }
	public double hdgK { get; set; }
	public double hdgsigmaC { get; set; }
	public double hdgT { get; set; }
	public double cogK { get; set; }
	public double cogsigmaC { get; set; }
	public double cogT { get; set; }
	public float vmax { get; set; } // m/s target top speed at full power, straight rudder
	public double inertia { get; set; }
	public double rudlpersec { get; set; }
	public double rotHdg { get; set; } = 0;
	public double rotCog { get; set; } = 0;

	// State
	public double latitudeDeg = 0;
	public double longitudeDeg = 0;
	public float headingRad = 0f;    // 0=N, +pi/2=E
	public float yawRate = 0f;       // rad/s

	// User inputs
	[Range(0, 1)] public float enginePower = 0f;
	[Range(-35, 35)] public float rudderDeg = 0f;

	// Tunning
	public float thrustForce = 5e5f;    // N at enginePower=1
	public float rudderMax = 35f;		// deg
	public float rudderDragK = 1.8f;    // extra drag multiplier for rudder angle^2 (1.0–3.0)
	public float yawGain = 0.6f;        // how strongly rudder turns the ship
	public float yawAccel = 0.8f;       // rad/s^2, how fast yawRate reaches target rate

	//Constants
	const float R_earth = 6371000;		// meters, earth radius
	const float EPS_COS = 1e-5f;        // avoid division by zero near poles

	public Vector3 CalculateV3(Ship ship)
	{
		float dt = Time.deltaTime;
		float mass = (float)ship.Weight;
		float enginePower = Mathf.Clamp01((float)ship.EnginePower);
		float rudderDeg = Mathf.Clamp((float)ship.Rudder, -35f, 35f);


		// ---------- Acceleration - thrust and drag ----------
		// Calculate drag so that: thrust ~= drag at vmax with straight rudder (vessel is not accelerating)
		// dragForce0 (rudder=0) = thrustForce (at vmax)
		// dragForce0 (rudder=0) = dragCoefficient0 * speed^2
		// dragCoefficient0 * speed^2 = thrustForce  -->  dragCoefficient0 = thrustForce / speed^2
		float dragCoefficient0 = (vmax != 0f) ? (thrustForce / (vmax * vmax)) : 0f;

		// Rudder-induced drag
		float rudderFrac = Mathf.Abs(rudderDeg) / rudderMax;
		float dragCoefficient = dragCoefficient0 * (1f + rudderDragK * rudderFrac * rudderFrac);

		// Speed drag
		float v = (float)ship.Speed;
		float dragForce = dragCoefficient * v * v * Mathf.Sign(v);	// frictional resistance proportional to v^2

		float actualThrustForce = thrustForce * enginePower;
		float netForce = actualThrustForce - dragForce;    // N
		float accel = netForce / mass;
		v += accel * dt;

		// Prevent tiny float tails and negative creep when power is zero
		if (enginePower <= 0.0001f && Mathf.Abs(v) < 0.01f)
			v = 0f;
		v = Mathf.Max(0f, v);

		ship.Speed = v;


		// ---------- Yaw / turning dynamics ----------
		// Simple empirical yaw model: desired yaw rate is proportional to rudder * (speed / length)
		float rudderNorm = (float)(rudderDeg / rudderMax); // -1..1
		float yawTarget = yawGain * rudderNorm * (v / Mathf.Max((float)ship.Length, 10f)); // rad/s
		yawRate = Mathf.MoveTowards(yawRate, yawTarget, yawAccel * dt);
		headingRad += yawRate * dt;		// update current heading

		// Wrap heading to [-pi, pi] for stability
		if (headingRad > Mathf.PI)
			headingRad -= 2f * Mathf.PI;
		else if (headingRad < -Mathf.PI)
			headingRad += 2f * Mathf.PI;

		// ---------- Spherical Earth position update ----------
		// Decompose ground speed into North/East components in meters/second.
		float vNorth = v * Mathf.Cos(headingRad);  // 0 rad = North
		float vEast = v * Mathf.Sin(headingRad);  // +pi/2 rad = East

		// Convert to radians for geographic math
		double latRad = latitudeDeg * Mathf.Deg2Rad;
		double cosLat = Math.Cos(latRad);
		double safeCos = Math.Max(EPS_COS, Math.Abs(cosLat)) * Math.Sign(cosLat);

		// Integrate latitude and longitude (note 1/cos factor for longitude)
		latRad += (vNorth / R_earth) * dt;
		double lonRad = longitudeDeg * Mathf.Deg2Rad;
		lonRad += (vEast / (R_earth * safeCos)) * dt;

		// Clamp latitude to just shy of the poles; wrap longitude to [-pi, pi]
		double maxLat = (90.0 - 1e-6) * Mathf.Deg2Rad;
		latRad = Math.Max(-maxLat, Math.Min(maxLat, latRad));
		lonRad = Math.IEEERemainder(lonRad, 2.0 * Math.PI); // wrap

		latitudeDeg = latRad * Mathf.Rad2Deg;
		longitudeDeg = lonRad * Mathf.Rad2Deg;


		// Normalized vector in course direction
		Vector3 course = Quaternion.Euler(0, Mathf.Rad2Deg * headingRad, 0) * Vector3.forward;

		ship.Cog = headingRad * Mathf.Rad2Deg;	// used temporarliy for vessel model rotation

		return Time.deltaTime * v * course;
	}

	public void Calculate(Ship ship)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		ship = null;
	}

}
