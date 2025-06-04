using Models.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
//using Converters;

namespace Models.Autopilots
{
    [Serializable]
    public class ClassicAutopilot : AutopilotBase, INotifyPropertyChanged
    {
        public double rudl_per_second { get; set; }

        private double getKdD(BindablePoint pointStart, BindablePoint pointEnd)
        {
          

            double xDiff = (pointEnd.X / 1852.0 / 60.0) - (pointStart.X / 1852.0 / 60.0);
            double yDiff = (pointEnd.Y / 1852.0 / 60.0) - (pointStart.Y / 1852.0 / 60.0);
            double yMedian = (pointStart.Y / 1852.0 / 60.0 + (pointEnd.Y / 1852.0 / 60.0)) / 2.0;

            double kdd = Math.Atan(xDiff * Math.Cos(yMedian / 60.0 * Math.PI / 180.0) / yDiff) * 180.0 / Math.PI;

            ////////////////////////////ogarnac
            if (yDiff <= 0 && xDiff > 0) { kdd += 180.0; }
            else if (yDiff <= 0 && xDiff < 0) { kdd += 180.0; }
            //if (yDi/ff >= 0 && xDiff < 0) { kdd += 360.0; }
            else if (yDiff > 0 && xDiff<0) { kdd += 360.0; }
            while (kdd >= 360)
            {
                kdd -= 360.0;
            }
            while (kdd < 0)
            {
                kdd += 360.0;
            }
            return kdd;

            /*
            decimal xDiff = (pointEnd.X / 1852.0m / 60.0m) - (pointStart.X / 1852.0m / 60.0m);
            decimal yDiff = (pointEnd.Y / 1852.0m / 60.0m) - (pointStart.Y / 1852.0m / 60.0m);
            decimal yMedian = (pointStart.Y / 1852.0m / 60.0m + (pointEnd.Y / 1852.0m / 60.0m)) / 2.0m;

            double kdd = Math.Atan(((double)xDiff) * Math.Cos(((double)yMedian)) / 60.0 * Math.PI / 180.0) / ((double)yDiff) * 180.0 / Math.PI;

            ////////////////////////////ogarnac
            if (yDiff <= 0 && xDiff > 0) { kdd += 180.0; }
            if (yDiff <= 0 && xDiff < 0) { kdd += 180.0; }
            //if (yDi/ff >= 0 && xDiff < 0) { kdd += 360.0; }
            if (yDiff > 0 && xDiff < 0) { kdd += 360.0; }
            return kdd;*/
        }

        public override void Calculate()
        {
            if(AutopilotMode == Enums.AutopilotModeEnum.none)
            {
                return;
            }
            if(AutopilotMode == Enums.AutopilotModeEnum.point)
            {
                getNewPreviousPoint();

                if (NextPoint != null)
                {
                    if(Math.Abs(NextPoint.X-previousPoint.X) < 300 && Math.Abs(NextPoint.Y - previousPoint.Y) < 300)
                    {
                        RoutePoints.RemoveAt(0);
                        NextPoint = null;
                    }
                }

                if (getNextPoint() != false)
                {
                    Course = getKdD(previousPoint, NextPoint);
                }
            }
       
            {
                double autopilot = Course, kurs;
                double _rot = ship.Rot / 60.0;

                    kurs = ship.Cog;

                    double roznica = 0;
                    if (Math.Abs(autopilot - kurs) > 180)
                    {
                        if (autopilot < kurs)
                        {
                            autopilot += 360;
                        }
                        else
                        {
                            kurs += 360;
                        }
                        roznica = autopilot - kurs - ship.Rudder;
                        roznica -= _rot * 8;

                        if (Math.Abs(roznica) > rudl_per_second)
                        {
                            if (roznica > 0)
                                roznica = rudl_per_second;
                            else roznica = 0 - rudl_per_second;
                        }

                        ship.Rudder = (ship.Rudder + Math.Round(roznica, 1));
                    }
                    else
                    {
                        roznica = autopilot - kurs - ship.Rudder;
                        roznica -= _rot * 8;

                        if (Math.Abs(roznica) > rudl_per_second)
                        {
                            if (roznica > 0)
                                roznica = rudl_per_second;
                            else roznica = 0 - rudl_per_second;
                        }

                        if (autopilot > kurs)
                        {

                        ship.Rudder=(ship.Rudder + Math.Round(roznica, 1));
                        }
                        else
                        {
                        ship.Rudder=(ship.Rudder + Math.Round(roznica, 1));
                        }
                    }

                
            }

            
        }

       
        private BindablePoint previousPoint { get; set; }
        private void getNewPreviousPoint()
        {
            previousPoint = new BindablePoint(ship.PosX, ship.PosY);
        }

        public override void SetAutopilotDefaultData(object obj)
        {
            rudl_per_second = (double)obj;
        }

       

        public override void Dispose()
        {
            ship = null;
        }
    }
}
