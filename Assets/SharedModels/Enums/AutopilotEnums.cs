using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Enums
{
    public enum AutopilotEnums
    {
        classic,
        none
    }

    public class AutopilotEnumsProvider
    {
        public static List<String> GetStrings()
        {
            return Enum.GetNames(typeof(AutopilotEnums)).ToList();
        }
    }
}
