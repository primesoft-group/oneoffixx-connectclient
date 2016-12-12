/* =============================================================================
 * Copyright (C) by Sevitec AG
 *
 * Project: OneOffixx.ConnectClient.WinApp.HistoryStore
 * 
 * =============================================================================
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OneOffixx.ConnectClient.WinApp.HistoryStore
{
    [XmlType(AnonymousType = true)]
    public class LogEntrys
    {
        public Guid LogGuid { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }

        [XmlElement(typeof(RequestEntry), ElementName = "RequestEntry")]
        public RequestEntry RequestEntry { get; set; }

        [XmlElement(typeof(ResponseEntry), ElementName = "ResponseEntry")]
        public ResponseEntry ResponseEntry { get; set; }
        public bool IsFavourite { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class RequestEntry
    {
        public string Uri { get; set; }

        public CData Username { get; set; }

        public CData Password { get; set; }

        public CData Content { get; set; }

        public DateTime Date { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ResponseEntry
    {
        public string StatusCode { get; set; }

        public string Filename { get; set; }

        public string TimeUsed { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class HistoryEntry
    {
        [XmlElement(typeof(LogEntrys), ElementName = "LogEntrys")]
        public List<LogEntrys> Logs { get; set; }
    }

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

    public class CData : IXmlSerializable
    {
        private string _value;

        /// <summary>
        /// Allow direct assignment from string:
        /// CData cdata = "abc";
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CData(string value)
        {
            return new CData(value);
        }

        /// <summary>
        /// Allow direct assigment to string
        /// string str = cdata;
        /// </summary>
        /// <param name="cdata"></param>
        /// <returns></returns>
        public static implicit operator string(CData cdata)
        {
            if (cdata == null) return null;

            return cdata._value;
        }

        public CData() : this(string.Empty)
        {
        }

        public CData(string value)
        {
            _value = value;
        }

        public bool IsNotNull()
        {
            return _value != null;
        }

        public override string ToString()
        {
            return _value;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _value = reader.ReadElementString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(_value);
        }
    }
}
