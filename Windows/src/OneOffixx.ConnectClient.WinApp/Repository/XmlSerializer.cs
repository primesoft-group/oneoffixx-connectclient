using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OneOffixx.ConnectClient.WinApp.Repository
{
    public static class XmlSerializer
    {
        public static T Deserialize<T>(string xml) where T : class
        {
            try
            {
                using (var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings() { CheckCharacters = false }))
                {
                    return Serializer<T>().CanDeserialize(reader)
                        ? Serializer<T>().Deserialize(reader) as T
                        : null;
                }
            }
            catch (XmlException)
            {
                return null;
            }
        }

        public static T Deserialize<T>(Stream stream) where T : class
        {
            try
            {
                using (var reader = new XmlTextReader(stream))
                {
                    return Serializer<T>().CanDeserialize(reader)
                        ? Serializer<T>().Deserialize(reader) as T
                        : null;
                }
            }
            catch (XmlException)
            {
                return null;
            }
        }

        private static System.Xml.Serialization.XmlSerializer Serializer<T>()
        {
            return new System.Xml.Serialization.XmlSerializer(typeof(T));
        }

        public static string Serialize<T>(T obj)
        {
            using (var sw = new StringWriter())
            {
                Serializer<T>().Serialize(sw, obj);
                return sw.ToString();
            }
        }

        public static string Serialize<T>(T obj, XmlWriterSettings settings)
        {
            if (settings == null)
                return Serialize(obj);

            using (var ms = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(ms, settings))
                {
                    Serializer<T>().Serialize(xmlWriter, obj);
                }

                return settings.Encoding.GetString(ms.ToArray());
            }
        }

        public static void Serialize<T>(Stream stream, T obj)
        {
            Serializer<T>().Serialize(stream, obj);
        }

        public static void Serialize<T>(TextWriter writer, T obj)
        {
            Serializer<T>().Serialize(writer, obj);
        }

        public static void Serialize<T>(XmlWriter writer, T obj)
        {
            Serializer<T>().Serialize(writer, obj);
        }

        /// <summary>
        /// returns a xml namespace from a type
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <returns>namespace</returns>
        public static string GetNamespace<T>()
        {
            return typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false).Cast<XmlRootAttribute>().Select(
                x => x.Namespace).FirstOrDefault();
        }
    }
}