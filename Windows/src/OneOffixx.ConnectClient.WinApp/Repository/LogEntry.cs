using System;
using System.Xml.Serialization;

namespace OneOffixx.ConnectClient.WinApp.Repository
{
    [XmlType(AnonymousType = true)]
    public class LogEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }

        [XmlElement(typeof(RequestEntry), ElementName = "Request")]
        public RequestEntry RequestEntry { get; set; }

        [XmlElement(typeof(ResponseEntry), ElementName = "Response")]
        public ResponseEntry ResponseEntry { get; set; }
        public bool IsFavorite { get; set; }
    }
}