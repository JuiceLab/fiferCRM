using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XMLModel.Sypexgeo
{
    [XmlRoot("city")]
    public class XMLCity : XMLBase
    {
        [XmlElement("lat")]
        public double Lat { get; set; }
        [XmlElement("lon")]
        public double Long { get; set; }
        [XmlElement("name_ru")]
        public double Name { get; set; }
        public XMLCity()
        { }
    }
}
