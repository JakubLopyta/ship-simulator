using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Models.Models
{
    public class ClassicModel : IModel, IDisposable
    {
        public ClassicModel() { }
        public ClassicModel(double _hdgK, double _hdgsigmaC, double _hdgT, double _cogK, double _cogsigmaC, double _cogT,
            double _posX, double _posY, double _speed, double _vmax, double _inertia, double _length, double _width, double _rudlpersec, double _rudlmax, Ship _ship)
        {
            hdgK = _hdgK;
            hdgsigmaC = _hdgsigmaC;
            hdgT = _hdgT;
            cogK = _cogK;
            cogsigmaC = _cogsigmaC;
            cogT = _cogT;
            vmax = _vmax;
            inertia = _inertia;
            rudlpersec = _rudlpersec;
            ship = _ship;
            ship.Length = _length;
            ship.Width = _width;
            ship.PosX = _posX;
            ship.PosY = _posY;
            ship.Speed = _speed;
            ship.RudlMax = _rudlmax;
        }

        public Ship ship { get; set; }
        public double hdgK { get; set; }
        public double hdgsigmaC { get; set; }
        public double hdgT { get; set; }
        public double cogK { get; set; }
        public double cogsigmaC { get; set; }
        public double cogT { get; set; }
        public double vmax { get; set; }
        public double inertia { get; set; }
        public double rudlpersec { get; set; }
        public double rotHdg { get; set; } = 0;
        public double rotCog{get;set;} = 0;

  

        public void Calculate(Ship ship)
        {

			// BM600DD1.xml model
			//_hdgK     _hdgsigmaC      _hdgT       _cogK       _cogsigmaC      _cogT       _posX _posY _speed _vmax _inertia   _length _width  _rudlpersec _rudlmax
			//0.090737d -0.079667512d   8.867580d   0.090794d   -0.07099909d    10.864230d  0d    0d    0d     20d   0.982d     70.7d   9d      4d          40d
			{
                rotHdg = (hdgK * (hdgsigmaC + ship.Rudder) + hdgT * rotHdg) / (1 + hdgT);
                ship.Hdg += rotHdg * 1; // dodaje wynik
            }
            {
                rotCog = (cogK * (cogsigmaC + ship.Rudder) + cogT * rotCog) / (1 + cogT);
                ship.Cog += rotCog * 1;
                ship.Hdg = ship.Cog; // nadpisuje ten wynik -> Hdg i Cog są cały czas takie same podczas ruchu statku
                ship.Rot = rotCog;
            }
;
            double pozycja_stopnie = ship.PosY / 1852.0 / 60.0;
            ship.PosY += 0.514444 * ship.Speed * 1 * Math.Cos(ship.Cog / 180 * Math.PI);    // 0.514444 - węzły -> m/s (oryginalny program działał co sekundę, w Unity co klatkę, więc statek porusza się bardzo szybko)
            if (Math.Abs(ship.PosY) > 1852 * 60 * 90)
                ship.PosY *= (-1);
            pozycja_stopnie += ship.PosY / 1852.0 / 60.0;
            pozycja_stopnie = pozycja_stopnie / 2.0;
            ship.PosX += (0.514444 * ship.Speed * 1 * Math.Sin(ship.Cog / 180 * Math.PI) / Math.Cos(pozycja_stopnie * Math.PI / 180));
            if (Math.Abs(ship.PosX) > 1852 * 60 * 180)
                ship.PosX *= (-1);
            ship.Speed = ship.Speed * (1 - (1)) + (ship.Speed * inertia + ship.EnginePower * (1 - inertia) * vmax) * 1;
            ship.Rot = rotHdg;
        }

        public void Dispose()
        {
            ship = null;
        }
    }
}
