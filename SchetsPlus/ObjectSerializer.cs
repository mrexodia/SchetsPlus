using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using System.IO.Compression;

namespace SchetsEditor
{
    //Met dank aan: http://www.albahari.com/nutshell/ch15.aspx voor leerzame voorbeelden
    public class ObjectSerializer
    {
        public static void SerializeToCompressedFile<T>(T o, string file)
        {
            using (FileStream fileStream = File.Create(file))
            {
                using (GZipStream gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal))
                {
                    using (XmlWriter xmlStream = XmlWriter.Create(gzipStream, new XmlWriterSettings { Indent = true }))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(xmlStream, o);
                    }
                }
            }
        }

        public static T DeserializeFromCompressedFile<T>(string file)
        {
            using (FileStream fileStream = File.OpenRead(file))
            {
                using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    return (T)serializer.ReadObject(gzipStream);
                }
            }
        }
    }
}
