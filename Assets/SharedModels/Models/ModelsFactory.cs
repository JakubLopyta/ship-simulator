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
					throw new NotImplementedException();
				case ModelEnum.test:
					throw new NotImplementedException();
				default:
					return null;
					
			}
		}
    }
}
