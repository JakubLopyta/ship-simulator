using Models.Autopilots;
using Models.Enums;
using Models.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.Timeline;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour, INotifyPropertyChanged, IDisposable
{
	#region properties
	[SerializeField] private string name = string.Empty;
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (name != value)
			{
				name = value;
				NotifyPropertyChanged();
			}
		}
	}
	[SerializeField] private bool adminMode = true;
	public bool AdminMode
	{
		get
		{
			return adminMode;
		}
		set
		{
			if (adminMode != value)
			{
				adminMode = value;
				NotifyPropertyChanged();
			}
		}
	}
	[SerializeField] private bool isSelected = false;
	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			if (isSelected != value)
			{
				isSelected = value;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double hdgRotation = 0;
	public double HdgRotation
	{
		get
		{
			return 0 - hdgRotation;
		}
		set
		{
			if (isSelected == true)
			{
				hdgRotation = value;
				NotifyPropertyChanged();
			}
			else
			{
				hdgRotation = 0;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double posXRotation = 0;
	public double PosXRotation
	{
		get
		{
			return posXRotation;
		}
		set
		{
			if (isSelected == true)
			{
				posXRotation = value;
				NotifyPropertyChanged();
			}
			else
			{
				posXRotation = 0;
				NotifyPropertyChanged();
			}
		}
	}
	
	[SerializeField]
	private double posYRotation = 0;
	public double PosYRotation
	{
		get
		{
			return posYRotation;
		}
		set
		{
			if (isSelected == true)
			{
				posYRotation = value;
				NotifyPropertyChanged();
			}
			else
			{
				posYRotation = 0;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private bool fromFile = false;
	public bool FromFile
	{
		get
		{
			return fromFile;
		}
		set
		{
			if (fromFile != value)
			{
				fromFile = value;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private int status = 0;
	public int Status
	{
		get
		{
			return status;
		}
		set
		{
			if (status != value)
			{
				status = value;
				NotifyPropertyChanged();
			}
		}
	}
	
	[SerializeField] private string userIp = string.Empty;
	public string UserIp
	{
		get
		{
			return userIp;
		}
		set
		{
			if (userIp != value)
			{
				userIp = value;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private string callSign = string.Empty;
	public string CallSign
	{
		get
		{
			return callSign;
		}
		set
		{
			if (callSign != value)
			{
				callSign = value;
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
			}
		}
	}

	/*public ShipOwnerDatas shipOwnerData { get; set; }
	public GeometricData geometridData { get; set; }
	public ManeuveringParameters maneuveringParameters { get; set; }
	public MachineData machineData { get; set; }*/

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
				HdgRotation = hdg;
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
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
				if (value > RudlMax)
				{
					value = RudlMax;
				}
				else if (value < rudlMin)
				{
					value = rudlMin;
				}
				rudder = value;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double width = 10;
	public double Width
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
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double length = 100;
	public double Length
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
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double speed = 0;
	public double Speed
	{
		get
		{
			return Math.Round(speed, 1);
		}
		set
		{

			if (speed != value)
			{
				speed = value;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double posY = 0;
	public double PosY
	{
		get
		{
			return posY;
		}
		set
		{
			value = Math.Round(value, 10);
			if (posY != value)
			{
				posY = value;
				PosYRotation = PosY;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double posX = 0;
	public double PosX
	{
		get
		{
			return posX;
		}
		set
		{
			value = Math.Round(value, 10);
			if (posX != value)
			{
				posX = value;
				PosXRotation = value;
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double enginePower = 0;
	public double EnginePower
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
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private double rudlMax = 35;
	public double RudlMax
	{
		get
		{
			return rudlMax;
		}
		set
		{
			if (rudlMax != value)
			{
				rudlMax = value;
				NotifyPropertyChanged();
			}
		}
	}

	double rudlMin
	{
		get
		{
			return RudlMax * (-1.0);
		}
	}

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
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
				if (model is ClassicModel && AutoPilot is ClassicAutopilot)
				{
					AutoPilot.SetAutopilotDefaultData((object)(model as ClassicModel).rudlpersec);
				}
				if (model is ClassicModel)
				{
					ModelMode = ModelEnum.classic;
				}
				else
				{
					ModelMode = ModelEnum.none;
				}
			}
		}
	}
	
	[SerializeField] private AutopilotBase autoPilot = null;
	public AutopilotBase AutoPilot
	{
		get
		{
			return autoPilot;
		}
		set
		{
			if (autoPilot != value)
			{

				autoPilot = value;
				if (autoPilot != null)
				{
					autoPilot.PrepareAutopilot(this);
				}
				NotifyPropertyChanged();
			}
		}
	}

	[SerializeField] private AutopilotEnums autopilotMode;
	public AutopilotEnums AutopilotMode
	{
		get
		{
			return autopilotMode;
		}
		set
		{
			if (autopilotMode != value)
			{
				autopilotMode = value;
				AutoPilot = AutopilotFactory.GetAutopilot(autopilotMode);
				if (model is ClassicModel && AutoPilot is ClassicAutopilot)
				{
					AutoPilot.SetAutopilotDefaultData((object)(model as ClassicModel).rudlpersec);
				}
				NotifyPropertyChanged();
			}
		}
	}

    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created

	public bool simulationRunning = false;

	

    void Start()
    {
        

        Model = ModelsFactory.GetModel(null, ModelEnum.classic, this);

        
    }


    void Update()
    {
		if (simulationRunning)
		{
			Step();
			transform.position = new Vector3((float)posX, 0f, (float)posY);
			transform.rotation = Quaternion.Euler(0f, (float)Cog, 0f);
			
		}
    }

    public void Step()
	{
		if (AutoPilot != null)
		{
			AutoPilot.Calculate();
		}
		if (Model != null)
		{
			Model.Calculate(this);
		}
	}
	public Ship()
	{

	}
	public Ship(string nrMmsi, AutopilotEnums autopilotType = AutopilotEnums.none, ModelEnum model = ModelEnum.none, XmlReader reader = null)
	{
		Model = ModelsFactory.GetModel(reader, model, this);
		AutoPilot = AutopilotFactory.GetAutopilot(autopilotType);
		AutopilotMode = autopilotType;
		if (AutoPilot != null)
		{
			AutoPilot.PrepareAutopilot(this);
		}



		if (autopilotType == AutopilotEnums.classic && model == ModelEnum.classic)
		{
			AutoPilot.SetAutopilotDefaultData((object)(Model as ClassicModel).rudlpersec);
		}
		MMSI = Convert.ToInt32(nrMmsi);
	}
	public event PropertyChangedEventHandler PropertyChanged;

	private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public string GetCurrentCoordinates()
	{
		return "Longitude: " + Math.Round((posX / 1852 / 60), 4).ToString("F4") + "   Latitude: " + Math.Round((posY / 1852 / 60), 4).ToString("F4");
    }
	public void Dispose()
	{
		AutoPilot.Dispose();
		AutoPilot = null;
	}
}
