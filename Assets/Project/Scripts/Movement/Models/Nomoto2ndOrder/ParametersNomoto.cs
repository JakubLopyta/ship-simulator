/// <summary>Stores parameters for Nomoto movement models. This is a temporary solution, the parameters will ultimately be stored in a JSON file</summary>
public static class ParametersNomoto
{
	private static float[] ParamProvider(ShipTypeEnum shipType)
	{
		float K, T1, T2, T3;
		switch (shipType)
		{
			// LNG carrier (based on 'Galea' vessel, L = 290m)
			// (Nomoto-type manoeuvring mathematical models and their applicability to simulation tasks - Serge Sutulo, C.Guedes Soares, 2024)
			case ShipTypeEnum.tanker_lng:
				K = 1.342f;
				T1 = 5.779f;
				T2 = 0.341f;
				T3 = 0.635f;
				break;

			// Cargo ship (Fast Cargo Vessel - Mariner class, L = 172m)
			// (Guidance and Control of Ocean Vehicles - Thor I Fossen 1994)
			case ShipTypeEnum.cargo_general:
				K = 3.868f;
				T1 = 5.643f;
				T2 = 0.373f;
				T3 = 0.885f;
				break;
			default:
				K = 1f;
				T1 = 5f;
				T2 = 0.3f;
				T3 = 0.6f;
				break;
		}
		return new float[] { K, T1, T2, T3 };
    }

	/// <summary>
	/// Provides appropriate <b>dimensionless</b> parameters values for certain vessel.
	/// <br></br>
	/// 1st order parameters are calculated based on 2nd order values with formula
	/// <code>T = T1 + T2 - T3</code>
	/// </summary>
	// Parameters are dimesionless to make them independent of vessel's speed.
	public static void Get(ShipTypeEnum shipType, out float K, out float T)
	{
		float T1, T2, T3;
		float[] params2ndOrder = ParamProvider(shipType);
		K = params2ndOrder[0];
		T1 = params2ndOrder[1];
		T2 = params2ndOrder[2];
		T3 = params2ndOrder[3];
		T = T1 + T2 - T3;
	}

	/// <summary>
	/// Provides appropriate <b>dimensionless</b> parameters values for certain vessel.
	/// </summary>
	// Parameters are dimesionless to make them independent of vessel's speed.
	public static void Get(ShipTypeEnum shipType, out float K, out float T1, out float T2, out float T3)
	{
		float[] params2ndOrder = ParamProvider(shipType);
		K = params2ndOrder[0];
		T1 = params2ndOrder[1];
		T2 = params2ndOrder[2];
		T3 = params2ndOrder[3];
	}


}
