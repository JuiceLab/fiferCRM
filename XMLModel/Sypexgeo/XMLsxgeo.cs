using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XMLModel.Sypexgeo
{
    [XmlRoot("sxgeo")]
    public class XMLsxgeo : XMLBase
    {
        public XMLip Ip { get; set; }
        public XMLsxgeo()
        { }
    }
}
