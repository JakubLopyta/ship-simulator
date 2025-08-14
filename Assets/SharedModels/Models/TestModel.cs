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
			double _posX, double _posY, double _speed, double _vmax, double _inertia, double _length, double _width, double _rudlpersec, double _rudlmax, Ship _ship)
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
	public double vmax { get; set; }
	public double inertia { get; set; }
	public double rudlpersec { get; set; }
	public double rotHdg { get; set; } = 0;
	public double rotCog { get; set; } = 0;

	public Vector3 currentVector = Vector3.zero; // pr¹d morski
	public Vector3 windVector = Vector3.zero;    // wiatr
	[Range(0f, 1f)]
	public float windEffect = 0.0f;              // podatnoœæ na wiatr

	public Vector3 CalculateV3(Ship ship)
	{
		float mass = (float)ship.Weight;
		float enginePower = Mathf.Clamp01((float)ship.EnginePower);
		float rudderDeg = Mathf.Clamp((float)ship.Rudder, -35f, 35f);

		float vmax = 8f;           // m/s target top speed at full power, straight rudder
		float thrustForce = 5e5f;  // engine thrust force in N at enginePower = 1
		float rudderMax = 35f;     // deg
		float rudderDragK = 1.8f;  // extra drag multiplier for rudder angle^2 (1.0–3.0)

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
		v += accel * Time.deltaTime;

		// Prevent tiny float tails and negative creep when power is zero
		if (enginePower <= 0.0001f && Mathf.Abs(v) < 0.01f)
			v = 0f;
		v = Mathf.Max(0f, v);
		// Optional: hard cap above vmax so waves/turns don’t overshoot too far
		// v = Mathf.Min(v, vmax * 1.2f);

		ship.Speed = v;


		// === ROTATION ===
		/*
		// Moment of Inertia for a cylinder --- 1/4(MR^2) + 1/12(ML^2)
		float moi = (float)((ship.Weight * ship.Width / 2 * ship.Width / 2) / 4
				   + (ship.Weight * ship.Length * ship.Length) / 12);
		*/

		// Wektor jednostkowy w kierunku kursu
		Vector3 course = Quaternion.Euler(0, (float)ship.Cog, 0) * Vector3.forward;

		return (course * (float)ship.Speed + currentVector + windEffect * windVector) * Time.deltaTime;
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
