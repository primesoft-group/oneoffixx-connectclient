using System;
using System.Xml.Serialization;

namespace OneOffixx.ConnectClient.WinApp.Repository
{
    [XmlType(AnonymousType = true)]
    public class RequestEntry
    {
        public string Uri { get; set; }

        public CData Username { get; set; }

        public CData Password { get; set; }

        public CData Content { get; set; }

        public DateTime Date { get; set; }
    }
}