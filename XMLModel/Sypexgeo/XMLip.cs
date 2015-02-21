using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XMLModel.Sypexgeo
{
    [XmlRoot("ip")]
    public class XMLip : XMLBase
    {
        [XmlElement("city")]
        public XMLCity City { get; set; }

        public XMLip()
        { }
        //[XmlElement("region")]
        //public XMLRegion Region { get; set; }
        //[XmlElement("country")]
        //public XMLCountry Country { get; set; }


    }
}
