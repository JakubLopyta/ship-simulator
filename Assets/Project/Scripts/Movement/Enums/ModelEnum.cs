using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Enums
{
    public enum ModelEnum
    {
        test,
        none
    }

    public class ModelEnumProvider
    {
        public static List<String> GetStrings()
        {
            return Enum.GetNames(typeof(ModelEnum)).ToList();
        }
    }
}
