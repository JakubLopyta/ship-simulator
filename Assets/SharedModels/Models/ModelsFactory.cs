using System;
using UnityEngine.UIElements;

namespace Models.Models
{
    public class ModelsFactory
    {
		/// <summary>
		/// Creates and attaches desired model script to a ship's game object.
		/// </summary>
		public static IModel GetModel(Ship ship, ShipTypeEnum shipType, ModelEnum modelType)
		{
			ship.Length = shipType switch
			{
				ShipTypeEnum.tanker_lng => 290f,	// 'Galea' vessel
				ShipTypeEnum.cargo_general => 172f,	//Fast Cargio Mariner class vessel
				_ => 100,
			};

			float K;
			switch (modelType)
			{
				case ModelEnum.nomoto1stOrder:
					var nomoto_1st = ship.gameObject.AddComponent<ModelNomoto1stOrder>();
					ParametersNomoto.Get(shipType, out K, out float T);
					nomoto_1st.SetNomotoParameters(K, T);
					return nomoto_1st;
				case ModelEnum.nomoto2ndOrder:
					var nomoto_2nd = ship.gameObject.AddComponent<ModelNomoto2ndOrder>();
					ParametersNomoto.Get(shipType, out K, out float T1, out float T2, out float T3);
					nomoto_2nd.SetNomotoParameters(K, T1, T2, T3);
					return nomoto_2nd;
				case ModelEnum.test:
					return ship.gameObject.AddComponent<TestModel>();
				default:
					return null;
					
			}
		}
    }
}
