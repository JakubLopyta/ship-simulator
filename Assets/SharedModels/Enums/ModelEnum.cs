using System;
using System.Collections.Generic;
using System.Linq;

public enum ModelEnum
{
    test,
    nomoto1stOrder,
    nomoto2ndOrder,
    none
}

public class ModelEnumProvider
{
    public static List<String> GetStrings()
    {
        return Enum.GetNames(typeof(ModelEnum)).ToList();
    }
}
