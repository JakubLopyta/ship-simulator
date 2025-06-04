using System;
using System.Collections.Generic;
using System.Text;
using Models.Enums;
using Models.Autopilots;

namespace Models.Autopilots
{
    public static class AutopilotFactory
    {
        public static AutopilotBase GetAutopilot(AutopilotEnums type)
        {
            switch (type)
            {
                case AutopilotEnums.classic:
                    return new ClassicAutopilot();
                case AutopilotEnums.none:
                    return new AutopilotBase();
            }
            return null;
        }
    }
}
