using System;
using OneOffixx.ConnectClient.WinApp.Repository;

namespace OneOffixx.ConnectClient.WinApp.ViewModel
{
    public class Request
    {
        public string Uri { get; set; }

        public CData Username { get; set; }

        public CData Password { get; set; }

        public CData Content { get; set; }

        public DateTime Date { get; set; }
    }
}
