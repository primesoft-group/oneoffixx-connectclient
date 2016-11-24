/* =============================================================================
 * Copyright (C) by Sevitec AG
 *
 * Project: OneOffixx.ConnectClient.WinApp.HistoryStore
 * 
 * =============================================================================
 * */

using System;

namespace OneOffixx.ConnectClient.WinApp.HistoryStore
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
