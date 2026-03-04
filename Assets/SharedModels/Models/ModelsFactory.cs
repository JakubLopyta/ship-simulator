using System;

namespace Models.Models
{
    public class ModelsFactory
    {
		/// <summary>
		/// Creates and attaches desired model script to a ship's game object.
		/// </summary>
		public static IModel GetModel(Ship ship, ShipTypeEnum shipType, ModelEnum modelType)
		{
			switch (modelType)
			{
				case ModelEnum.nomoto1stOrder:
					throw new NotImplementedException();
				case ModelEnum.nomoto2ndOrder:
					var nomoto = ship.gameObject.AddComponent<Nomoto2ndOrderModel>();
					float[] nomotoParams = ParamNomoto.GetParameters2ndOrder(shipType);
					nomoto.SetNomotoParameters(nomotoParams[0], nomotoParams[1], nomotoParams[2], nomotoParams[3]);
					return nomoto;
				case ModelEnum.test:
					throw new NotImplementedException();
				default:
					return null;
					
			}
		}
    }
}
