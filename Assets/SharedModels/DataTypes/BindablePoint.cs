using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Models.DataTypes
{
    public class BindablePoint: INotifyPropertyChanged
    {
        public BindablePoint(double _x, double _y)
        {
            X = _x;
            Y = _y;
        }
        public BindablePoint() { }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private double x = 0, y = 0;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                value = Math.Round(value, 10);
                if (value != x)
                {
                    x = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                value = Math.Round(value, 10);
                if (y != value)
                {
                    y = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
