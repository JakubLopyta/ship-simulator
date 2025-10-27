using Models.Enums;
using Models.Models;
using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
	#region properties
	
	// Identification
	[SerializeField] private string shipName = string.Empty;
	public string ShipName
	{
		get
		{
			return shipName;
		}
		set
		{
			if (shipName != value)
			{
				shipName = value;
			}
		}
	}

	[SerializeField] private int mmsi;
	public int MMSI
	{
		get
		{
			return mmsi;
		}
		set
		{
			if (mmsi != value)
			{
				mmsi = value;
			}
		}
	}

	// Parameters
	[SerializeField] private float width = 10;
	public float Width
	{
		get
		{
			return width;
		}
		set
		{
			if (width != value)
			{
				width = value;
			}
		}
	}

	[SerializeField] private float length = 100;
	public float Length
	{
		get
		{
			return length;
		}
		set
		{
			if (length != value)
			{
				length = value;
			}
		}
	}

	[SerializeField] private uint weight = 5000000;
	public uint Weight
	{
		get
		{
			return weight;
		}
		set
		{
			if (weight != value)
			{
				weight = value;
			}
		}
	}

	[SerializeField] private float vmax = 8;
	public float Vmax
	{
		get
		{
			return vmax;
		}
		set
		{
			if (vmax != value)
			{
				vmax = value;
			}
		}
	}

	// State
	public double LatitudeDeg = 0;
	public double LongitudeDeg = 0;
	public double EcefX;
	public double EcefY;
	public double EcefZ;


	[SerializeField] private double hdg = 0;
	public double Hdg
	{
		get
		{
			return Math.Round(hdg, 2);
		}
		set
		{
			if (hdg != value)
			{
				hdg = value;
				while (hdg >= 360.0)
				{
					hdg -= 360.0;
				}
				while (hdg < 0)
				{
					hdg += 360;
				}
			}
		}
	}

	[SerializeField] private double cog = 0;
	public double Cog
	{
		get
		{
			return Math.Round(cog, 2);
		}
		set
		{
			if (cog != value)
			{
				cog = value;
				while (cog >= 360.0)
				{
					cog -= 360.0;
				}
				while (cog < 0)
				{
					cog += 360;
				}
			}
		}
	}

	[SerializeField] private double sog = 0;
	public double Sog
	{
		get
		{
			return Math.Round(sog, 1);
		}
		set
		{
			if (sog != value)
			{
				sog = value;
			}
		}
	}

	[SerializeField] private double rot = 0;
	public double Rot
	{
		get
		{
			return rot;
		}
		set
		{
			if (rot != value)
			{
				rot = value;
			}
		}
	}

	[SerializeField] private double rudder = 0;
	public double Rudder
	{
		get
		{
			return Math.Round(rudder, 2);
		}
		set
		{
			if (rudder != value)
			{
				rudder = value;
			}
		}
	}

	

	[SerializeField] private double speed = 0;
	public double Speed
	{
		get
		{
			return speed;
		}
		set
		{

			if (speed != value)
			{
				speed = value;
			}
		}
	}

	[SerializeField] private float enginePower = 0;
	public float EnginePower
	{
		get
		{
			return enginePower;
		}
		set
		{
			if (value > 1)
				value = 1;
			if (value < -1)
				value = -1;
			if (enginePower != value)
			{
				enginePower = value;
			}
		}
	}

	//Model
	[SerializeField] private ModelEnum modelMode = ModelEnum.none;
	public ModelEnum ModelMode {
		get
		{
			return modelMode;
		}
		set
		{
			if (modelMode != value)
			{
				modelMode = value;
			}
		}
	}
	[SerializeField] private IModel model;
	public IModel Model
	{
		get
		{
			return model;
		}
		set
		{
			if (model != value)
			{
				model = value;
				if (model is TestModel)
				{
					ModelMode = ModelEnum.test;
				}
				else
				{
					ModelMode = ModelEnum.none;
				}
			}
		}
	}

	public bool testModel = true;
	public bool simulationRunning = false;
	#endregion

	void Start()
	{
		CoordinatesConversion.GeodeticToEcef(
			LatitudeDeg, LongitudeDeg, 0,
			out EcefX, out EcefY, out EcefZ);

		if (OriginManager.Instance == null)
			Debug.Log("Ship: OriginManager not found");
		else
			OriginManager.Instance.Initialize(this);

		if (testModel)
			Model = ModelsFactory.GetModel(ModelEnum.test, this);
	}


    void Update()
    {
		if (simulationRunning)
		{
			Step();
		}
    }

    public void Step()
	{
		if (testModel)
		{
			Model.Calculate();
		}
	}
}
