using Models.Models;
using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
	#region Properties

	#region Identification
	private string shipName = string.Empty;
	public string ShipName
	{
		get => shipName;
		set
		{
			if (shipName == value) return;
			shipName = value;
		}
	}

	private int mmsi;
	public int MMSI
	{
		get => mmsi;
		set
		{
			if (mmsi == value) return;
			mmsi = value;
		}
	}

	private string callSign;
	public string CallSign
	{
		get => callSign;
		set
		{
			if (callSign == value) return;
			callSign = value;
		}
	}
	#endregion

	#region Geometric data
	[Header("Geometric data")]
	private float breadth = 10; // width, also "beam" [m]
	public float Breadth
	{
		get => breadth;
		set
		{
			if (breadth == value) return;
			breadth = value;
		}
	}

	[SerializeField] private float length = 200; // [m]
	public float Length
	{
		get => length;
		set
		{
			if (length == value) return;
			length = value;
		}
	}

	private float draft = 8; // also "draught" [m]
	public float Draft
	{
		get => draft;
		set
		{
			if (draft == value) return;
			draft = value;
		}
	}

	private float blockCoefficient = 0.75f; // 0-1
	public float BlockCoefficient
	{
		get => blockCoefficient;
		set
		{
			if (blockCoefficient == value) return;
			blockCoefficient = value;
		}
	}

	[SerializeField] private uint displacement = (uint)20e6; // weight of ship [kg]
	public uint Displacement
	{
		get => displacement;
		set
		{
			if (displacement == value) return;
			displacement = value;
		}
	}
	#endregion

	#region Other parameters
	[Header("Other parameters")]
	[SerializeField] private float vmax = 8; // [m/s]
	public float Vmax
	{
		get => vmax;
		set
		{
			if (vmax == value) return;
			vmax = value;
		}
	}
	private float rudderMax = 35f; // [deg]
	public float RudderMax
	{
		get => rudderMax;
		set
		{
			if(rudderMax == value) return;
			rudderMax = value;
		}
	}
	#endregion

	#region Coordinates
	[Header("Coordinates")]
	public double LatitudeDeg = 0;
	public double LongitudeDeg = 0;
	public double Height = 0;
	public double EcefX;
	public double EcefY;
	public double EcefZ;
	#endregion

	#region State
	[Header("State")]
	[SerializeField] [Range(0,360)] private double hdg = 0;
	public double Hdg
	{
		get => hdg;
		set
		{
			if (hdg == value) return;
			hdg = value;
			while (hdg >= 360.0)
				hdg -= 360.0;
			while (hdg < 0.0)
				hdg += 360.0;
		}
	}

	[SerializeField] [Range(0,360)] private double cog = 0;
	public double Cog
	{
		get => cog;
		set
		{
			if (cog == value) return;
			cog = value;
			while (cog >= 360.0)
				cog -= 360.0;
			while (cog < 0)
				cog += 360;
		}
	}

	[SerializeField] private double sog = 0;
	public double Sog
	{
		get => sog;
		set
		{
			if (sog == value) return;
			sog = value;
		}
	}

	[SerializeField] private double rot = 0;
	public double Rot
	{
		get => rot;
		set
		{
			if (rot == value) return;
			rot = value;
		}
	}

	[SerializeField] private double speed = 0;
	public double Speed
	{
		get => speed;
		set
		{

			if (speed == value) return;
			speed = value;
		}
	}

	[SerializeField] [Range(0,1)] private float enginePower = 0;
	public float EnginePower
	{
		get => enginePower;
		set
		{
			value = Mathf.Clamp01(value);
			if(enginePower == value) return;
			enginePower = value;
		}
	}

	[SerializeField] [Range(-35,35)] private double rudder = 0;
	public double Rudder
	{
		get => Math.Round(rudder, 2);
		set
		{
			value = Mathf.Clamp((float)value, -RudderMax, RudderMax);
			if (rudder == value) return;
			Rudder = value;
		}
	}
	#endregion

	#region Model
	[Space]
	[SerializeField] private ModelEnum modelMode = ModelEnum.none;
	public ModelEnum ModelMode
	{
		get => modelMode;
		set
		{
			if (modelMode == value) return;
			modelMode = value;
		}
	}
	private IModel model;
	public IModel Model
	{
		get => model;
		set
		{
			if (model == value) return;
			model = value;
		}
	}
	#endregion
	private ShipTypeEnum shipType;
	public ShipTypeEnum ShipType
	{
		get => shipType;
		set
		{
			if(shipType == value) return;
			shipType = value;
		}
	}

	public bool simulationRunning = false;

	#endregion Properties

	void Start()
	{
		CoordinatesConversion.GeodeticToEcef(
			LatitudeDeg, LongitudeDeg, Height,
			out EcefX, out EcefY, out EcefZ);

		if (OriginManager.Instance == null)
			Debug.Log("Ship: OriginManager not found");
		else
			OriginManager.Instance.Initialize(this);

		switch (ModelMode)
		{
			case ModelEnum.nomoto1stOrder:
				Model = ModelsFactory.GetModel(this, ShipType, ModelEnum.nomoto1stOrder);
				break;
			case ModelEnum.nomoto2ndOrder:
				Model = ModelsFactory.GetModel(this, ShipType, ModelEnum.nomoto2ndOrder);
				break;
		}
	}

	public void ResetState(float speed = 0)
	{
		Model.ResetState();
		Rudder = 0;
		Speed = speed;
	} 
}
