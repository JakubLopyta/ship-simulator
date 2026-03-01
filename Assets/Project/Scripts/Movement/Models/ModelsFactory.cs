using Models.Enums;

namespace Models.Models
{
    public class ModelsFactory
    {
        public static IModel GetModel(ModelEnum modelType, Ship ship)
        {
            switch (modelType)
            {
				case ModelEnum.test:
					return new TestModel(ship);
			}
            return null;
        }
    }
}
