using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace EQS.KMS.Application.Encryption
{
    public class SerializerLogic
    {
        public static string SerializeObject(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static object Deserialize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            return binaryFormatter.Deserialize(memoryStream);
        }

        public static string XmlSerializeObject(object obj, string xmlRootAttributeName = null)
        {
            Type objectType = obj.GetType();
            XmlSerializer ser;

            if (!string.IsNullOrWhiteSpace(xmlRootAttributeName))
            {
                XmlRootAttribute xmlRootAttribute = new System.Xml.Serialization.XmlRootAttribute(xmlRootAttributeName);
                ser = new XmlSerializer(objectType, xmlRootAttribute);
            }
            else
                ser = new XmlSerializer(objectType);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, obj);

            return sb.ToString(); //.Replace("utf-16", "utf-8");
        }

        public static string XmlSerialize<T>(T obj)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, obj);

            return sb.ToString(); //.Replace("utf-16", "utf-8");
        }

        public static object XmlDeserializeObject(Type type, string xmlString, string xmlRootAttributeName = null)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                return null;

            XmlSerializer ser;

            if (!string.IsNullOrWhiteSpace(xmlRootAttributeName))
            {
                XmlRootAttribute xmlRootAttribute = new System.Xml.Serialization.XmlRootAttribute(xmlRootAttributeName);
                ser = new XmlSerializer(type, xmlRootAttribute);
            }
            else
                ser = new XmlSerializer(type);

            StringReader reader = new StringReader(xmlString);

            return ser.Deserialize(reader);
        }

        public static T XmlDeserialize<T>(string xmlString) where T : class
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                return null;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xmlString);
            T o = ser.Deserialize(reader) as T;

            return o;
        }
    }
}
