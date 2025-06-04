using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Models.Enums;
using Models.DataTypes;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Models.Autopilots
{
    [XmlInclude(typeof(ClassicAutopilot))]
    [Serializable]
    public class AutopilotBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private AutopilotModeEnum autopilotMode = AutopilotModeEnum.none;
        

        public AutopilotModeEnum AutopilotMode
        {
            get
            {
                return autopilotMode;
            }
            set
            {
                if (autopilotMode != value)
                {
                    autopilotMode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<BindablePoint> RoutePoints { get; set; } = new ObservableCollection<BindablePoint>();

        public bool getNextPoint()
        {
            if(RoutePoints==null || RoutePoints.Count==0)
            {
                return false;
            }
            NextPoint = RoutePoints[0];
            return true;
        }

        private BindablePoint nextPoint = null;
        public BindablePoint NextPoint
        {
            get
            {
                return nextPoint;
            }
            set
            {
                if (nextPoint != value)
                {
                    nextPoint = value;
                    NotifyPropertyChanged();
                }
            }
        }


        protected Ship ship { get; set; }
        
        private double course = 0;
        public virtual double Course
        {
            get
            {
                return course;
            }
            set
            {
                if (value >= 360.0)
                {
                    value -= 360;
                }
                if(value<0.0)
                {
                    value += 360.0;
                }
                if (value != course)
                {
                    course = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public virtual void Calculate()
        {
        }

        public virtual void AddPoint(double x, double y)
        {
            RoutePoints.Add(new BindablePoint(x, y));
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
        
        
        public virtual void PrepareAutopilot(Ship _ship)
        {
            ship = _ship;
        }
        
        public virtual void SetAutopilotDefaultData(object obj)
        {
            throw new NotImplementedException();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
