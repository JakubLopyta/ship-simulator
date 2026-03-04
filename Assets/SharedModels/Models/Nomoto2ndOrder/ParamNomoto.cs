/// <summary>Stores information about parameters for Nomoto 1st and 2nd order movement model.</summary>
public static class ParamNomoto
{
	/// <summary>Provides appropriate <b>dimensionless</b> parameters values for certain vessel.</summary>
	/// <returns>Array of dimensionless parameters { K, T1, T2, T3 }</returns>
	// Parameters are dimesionless to make them independent of vessel's speed.
	public static float[] GetParameters2ndOrder(ShipTypeEnum shipType)
	{
		float K, T1, T2, T3;
		switch (shipType)
		{
			// LNG carrier (from 'Galea' ship, L = 290m)
			case ShipTypeEnum.tanker_lng:
				K = 1.342f;
				T1 = 5.779f;
				T2 = 0.341f;
				T3 = 0.635f;
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
	/// <returns>Array of dimensionless parameters { K, T }</returns>
	// Parameters are dimesionless to make them independent of vessel's speed.
	public static float[] GetParameters1stOrder(ShipTypeEnum shipType)
	{
		float K, T1, T2, T3;
		float[] params2ndOrder = GetParameters2ndOrder(shipType);
		K = params2ndOrder[0];
		T1 = params2ndOrder[1];
		T2 = params2ndOrder[2];
		T3 = params2ndOrder[3];
		T1 = T1 + T2 - T3;
		return new float[] { K, T1 };
	}


}
