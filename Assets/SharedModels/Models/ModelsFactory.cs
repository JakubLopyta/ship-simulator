using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Models.Enums;

namespace Models.Models
{
    public class ModelsFactory
    {

        ///(string mmsi, string cogk, string cogt, string cogsigmac, string hdgk, string hdgt, string hdgsigmac, string width, string length, string inertia, string vmax, string rudlmax, string rudlpersecond)
        public static IModel GetModel(XmlReader doc, ModelEnum modelType, Ship ship)
        {
            switch (modelType)
            {
                case ModelEnum.classic:
                    try
                    {
                        //Hardcoded from BM600DD1.xml model
                        //                      _hdgK,     _hdgsigmaC,   _hdgT,      _cogK,     _cogsigmaC,   _cogT,      _posX, _posY, _speed, _vmax, _inertia, _length, _width, _rudlpersec, _rudlmax, _ship
                        return new ClassicModel(0.090737d, -0.079667512d, 8.867580d, 0.090794d, -0.07099909d, 10.864230d, 0d, 0d, 0d, 20d, 0.982d, 70.7d, 9d, 4d, 40d, ship);
					}
					catch (Exception ex)
                    {
                    }
                    break;
				case ModelEnum.test:
					try
					{
						//Hardcoded from BM600DD1.xml model
						//                      _hdgK,     _hdgsigmaC,   _hdgT,      _cogK,     _cogsigmaC,   _cogT,      _posX, _posY, _speed, _vmax, _inertia, _length, _width, _rudlpersec, _rudlmax, _ship
						return new TestModel(0.090737d, -0.079667512d, 8.867580d, 0.090794d, -0.07099909d, 10.864230d, 0d, 0d, 0d, 20d, 0.982d, 70.7d, 9d, 4d, 40d, ship);
					}
					catch (Exception ex)
					{
					}
					break;
			}
            return null;
        }

        /* Original GetModel method with XmlReading from file
        public static IModel GetModel(XmlReader doc, ModelEnum modelType, Ship ship)
        {
            switch (modelType)
            {
                case ModelEnum.classic:
                    try
                    {

                        string[] xmlStrings = new string[13];
                        doc.MoveToContent();
                        int licznik = 0;
                        while (doc.Read())
                        {
                            switch (doc.NodeType)
                            {

                                case XmlNodeType.Text:

                                    xmlStrings[licznik] = Normalizuj(doc.Value.ToString());
                                    ++licznik;
                                    break;
                            }
                        }
                        doc.Dispose();
                        return new ClassicModel(Convert.ToDouble(xmlStrings[8]), Convert.ToDouble(xmlStrings[10]), Convert.ToDouble(xmlStrings[9]), Convert.ToDouble(xmlStrings[3]), Convert.ToDouble(xmlStrings[5]),
                            Convert.ToDouble(xmlStrings[4]), 0, 0, 0, Convert.ToDouble(xmlStrings[7]), Convert.ToDouble(xmlStrings[6]), Convert.ToDouble(xmlStrings[2]), Convert.ToDouble(xmlStrings[1]),
                            Convert.ToDouble(xmlStrings[12]), Convert.ToDouble(xmlStrings[11]), ship);

                    }
                    catch (Exception ex)
                    {

                    }
                    break;
            }
            *//*
            int i=comboBox1.SelectedIndex;
            matka.dodaj_nowy_statek(
                mmsi,
                xml_wczytane[i,3],cogk
                xml_wczytane[i,4],cogt
                xml_wczytane[i,5],cogsigmac
                xml_wczytane[i,8],hdgk
                xml_wczytane[i,9],hdgt
                xml_wczytane[i,10],hdgsigmac
                xml_wczytane[i,1],width
                xml_wczytane[i,2],length
                xml_wczytane[i,6],inertia
                xml_wczytane[i,7],vmax
                xml_wczytane[i,11],rudlmax
                xml_wczytane[i,12]rudlpersec
                );
            this.Close();
        *//*
            return null;
		}*/


        private static string Normalizuj(string input)
        {
            input = input.Replace('.', ',');
            return input;
        }
    }
}
