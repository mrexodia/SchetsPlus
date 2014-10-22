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
        public static void SerializeToFile<T>(T o, string file)
        {
            using (XmlWriter xmlStream = XmlWriter.Create(file, new XmlWriterSettings { Indent = true }))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(xmlStream, o);
            }
        }

        public static T DeserializeFromFile<T>(string file)
        {
            using (Stream fileStream = File.OpenRead(file))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(fileStream);
            }
        }
    }
}
