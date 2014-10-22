using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;

namespace SchetsEditor
{
    //Met dank aan: http://www.albahari.com/nutshell/ch15.aspx voor leerzame voorbeelden
    public class ObjectSerializer
    {
        public static string Serialize<T>(T o)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, o);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static T Deserialize<T>(string s)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, Encoding.UTF8, new XmlDictionaryReaderQuotas(), null);
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(reader);
            }
        }
    }
}
