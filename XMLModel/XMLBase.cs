using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XMLModel
{
    public class XMLBase
    {
        public string ToXML(XmlAttributeOverrides xmlAttributeOverrides = null, Encoding Encode = null)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlWriterSettings.Indent = true;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var sw = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    var s = (xmlAttributeOverrides == null)
                                 ? new XmlSerializer(this.GetType())
                                 : new XmlSerializer(this.GetType(), xmlAttributeOverrides);

                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    s.Serialize(sw, this, namespaces);
                    sw.Flush();
                    sw.Close();

                    return Encode != null ?
                        Encode.GetString(memoryStream.ToArray())
                        : Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        public static T FromXML<T>(string xml)
        {
            using (var sw = new StringReader(xml))
            {
                var obj = Activator.CreateInstance<T>();
                var s = new XmlSerializer(obj.GetType());

                return (T)s.Deserialize(sw);
            }
        }

        public virtual object FromXML(string xml)
        {
            using (var sw = new StringReader(xml))
            {
                var s = new XmlSerializer(this.GetType());

                return s.Deserialize(sw);
            }
        }
    }
}
