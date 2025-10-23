using Models.Models;
using System;
using UnityEngine;

public class TestModel : IModel
{
	public TestModel(Ship _ship)
	{
		ship = _ship;
	}

	// Ship parameters
	public Ship ship;
	public float vmax;              // m/s target top speed at full power, straight rudder
	public float length;
	public uint mass;

	// State
	public float headingRad = 0f;    // 0=N, +pi/2=E
	public float yawRate = 0f;       // rad/s

	// User inputs
	[Range(0, 1)] public float enginePower = 0f;
	[Range(-35, 35)] public float rudderDeg = 0f;

	// Tunning
	public float thrustForce = 5e5f;    // N at enginePower=1
	public float rudderMax = 35f;		// deg
	public float rudderDragK = 1.8f;    // extra drag multiplier for rudder angle^2 (1.0-3.0)
	public float yawGain = 0.5f;        // how strongly rudder turns the ship
	public float yawAccel = 5e-3f;      // rad/s^2, how fast yawRate reaches target rate

	// TEMPORARY FOR GEODETIC
	public double height = 0;
	public void Calculate()
	{
		float dt = Time.deltaTime;
		vmax = ship.Vmax;
		mass = (uint)Mathf.Clamp(ship.Weight, 1000, 1000000000);   // minimum one tonne, maximum milion tonns
		length = ship.Length;
		enginePower = ship.EnginePower;
		rudderDeg = Mathf.Clamp((float)ship.Rudder, -35f, 35f);
		

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
		float yawTarget = yawGain * rudderNorm * (v / Mathf.Max(length, 10f)); // rad/s
		yawRate = Mathf.MoveTowards(yawRate, yawTarget, yawAccel * dt);
		headingRad += yawRate * dt;		// update current heading

		// Wrap heading to [-pi, pi] for stability
		if (headingRad > Mathf.PI)
			headingRad -= 2f * Mathf.PI;
		else if (headingRad < -Mathf.PI)
			headingRad += 2f * Mathf.PI;


		// ---------- Spherical Earth position update ----------
		double ecef_x, ecef_y, ecef_z;
		CoordinatesConversion.GeodeticToEcef(ship.LatitudeDeg, ship.LongitudeDeg, height, out ecef_x, out ecef_y, out ecef_z);

		// TODO: ORIGIN POINT
		double xEast, yNorth, zUp;
		CoordinatesConversion.EcefToEnu(ecef_x, ecef_y, ecef_z, 0, 0 ,0, out xEast, out yNorth, out zUp);
				
		// Decompose ground speed into North/East components in meters/second.
		float vNorth = v * Mathf.Cos(headingRad);  // 0 rad = North
		float vEast = v * Mathf.Sin(headingRad);  // +pi/2 rad = East
		
		xEast += vEast * dt;
		yNorth += vNorth * dt;
		

		// --------- Update ship state ----------
		// Cog and Hdg are temporarily the same
		ship.Cog = headingRad * Mathf.Rad2Deg;
		ship.Hdg = ship.Cog;
		ship.Rot = yawRate * Mathf.Rad2Deg;
		ship.Sog = ship.Speed;

		Vector3 unityPos = new Vector3((float)xEast, (float)zUp, (float)yNorth);
		Quaternion unityRotation = Quaternion.Euler(0f, (float)ship.Cog, 0f);
		ship.transform.SetPositionAndRotation(unityPos, unityRotation);

		CoordinatesConversion.EnuToEcef(xEast, yNorth, zUp, 0, 0, 0, out ecef_x, out ecef_y, out ecef_z);
		CoordinatesConversion.EcefToGeodetic(ecef_x, ecef_y, ecef_z, out ship.LatitudeDeg, out ship.LongitudeDeg, out height);
	}
}
