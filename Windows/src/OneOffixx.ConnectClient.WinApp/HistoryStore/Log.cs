/* =============================================================================
 * Copyright (C) by Sevitec AG
 *
 * Project: OneOffixx.ConnectClient.WinApp.HistoryStore
 * 
 * =============================================================================
 * */

using OneOffixx.ConnectClient.WinApp.Helpers;
using System.Text;
using System.Windows.Input;

namespace OneOffixx.ConnectClient.WinApp.HistoryStore
{
    public class Log
    {
        private const string Green = "#19E866";
        private const string Red = "#FF0000";
        private const string ServerIcon = "Cloud";
        private const string ClientIcon = "Desktop";

        public ICommand DeleteValue { get; set; }
        public ICommand LoadHistory { get; set; }

        public Log(ViewModel.RequestViewModel viewmodel)
        {
            LoadHistory = new RelayCommand(viewmodel.LoadValues, param => true);
            DeleteValue = new RelayCommand(viewmodel.ExecuteDeleteValue, param => true);
        }

        public Log()
        {
        }

        public string Action { get; set; }
        public Request RequestEntry { get; set; }
        public Response ResponseEntry { get; set; }

        public string ResponseColor
        {
            get
            {
                if (ResponseEntry.StatusCode == "200")
                {
                    return Green;
                }
                else
                {
                    return Red;
                }
            }
        }

        public string HistoryToolTip
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Status: " + this.ResponseEntry.StatusCode + "\r");
                sb.Append("Filename: " + this.ResponseEntry.Filename + "\r");
                sb.Append("Time Used in sec: " + this.ResponseEntry.TimeUsed);

                return sb.ToString();
            }
        }

        public string ActionIcon
        {
            get
            {
                if (Action == "Server")
                {
                    return ServerIcon;
                }
                else
                {
                    return ClientIcon;
                }
            }
        }
    }
}
