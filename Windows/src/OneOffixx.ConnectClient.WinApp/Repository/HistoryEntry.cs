using System.Collections.Generic;
using System.Xml.Serialization;

namespace OneOffixx.ConnectClient.WinApp.Repository
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class HistoryEntry
    {
        [XmlElement(typeof(LogEntry), ElementName = "LogEntry")]
        public List<LogEntry> Logs { get; set; }
    }
}