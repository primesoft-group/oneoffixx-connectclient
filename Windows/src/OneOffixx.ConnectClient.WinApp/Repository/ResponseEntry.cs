using System.Xml.Serialization;

namespace OneOffixx.ConnectClient.WinApp.Repository
{
    [XmlType(AnonymousType = true)]
    public class ResponseEntry
    {
        public string StatusCode { get; set; }

        public string Filename { get; set; }

        public string TimeUsed { get; set; }
    }
}