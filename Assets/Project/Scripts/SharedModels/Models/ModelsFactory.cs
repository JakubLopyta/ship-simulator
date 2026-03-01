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
					//public TestModel(float _vmax, double _length, double _width, Ship _ship)
					return new TestModel(8f, 70d, 10d, ship);
			}
            return null;
        }
    }
}
