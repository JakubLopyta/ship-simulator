using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Enums
{
    public enum AutopilotModeEnum
    {
        none,
        course,
        point
    }

    public class AutopilotModeEnumProvider
    {
        public static List<String> GetStrings()
        {
            return Enum.GetNames(typeof(AutopilotModeEnum)).ToList();
        }
    }
}
