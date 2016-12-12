/* =============================================================================
 * Copyright (C) by Sevitec AG
 *
 * Project: OneOffixx.ConnectClient.WinApp.Model
 * 
 * =============================================================================
 * */

using OneOffixx.ConnectClient.WinApp.HistoryStore;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OneOffixx.ConnectClient.WinApp.Model
{
    public class RequestModel : INotifyPropertyChanged
    {
        private string username;
        private string password;
        private string url;
        private string directory;
        private string xmlString;
        private bool canExecute;
        private int selectedIndex;
        private ObservableCollection<Log> log;
        private ObservableCollection<Log> favouriteLog;
        private string error;
        private bool canExecuteClient;
        private System.Windows.Visibility visibility;
        private string errorMessage;

        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                RaisePropertyChanged("Error");
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    RaisePropertyChanged("SelectedIndex");
                }
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (username != value)
                {
                    username = value;
                    RaisePropertyChanged("Username");
                }
            }
        }

        public System.Windows.Visibility WarningVisibility
        {
            get
            {
                return visibility;
            }
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    RaisePropertyChanged("WarningVisibility");
                }
            }
        }

        public string WarningMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    RaisePropertyChanged("WarningMessage");
                }
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password != value)
                {
                    password = value;
                    RaisePropertyChanged("Password");
                }
            }
        }

        public ObservableCollection<Log> Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
                FavouriteLog = new ObservableCollection<Log>(log.Where(x => x.IsFavourite));
                RaisePropertyChanged("Log");
            }
        }

        public ObservableCollection<Log> FavouriteLog
        {
            get
            {
                return favouriteLog;
            }
            set
            {
                favouriteLog = value;
                RaisePropertyChanged("FavouriteLog");
            }
        }

        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                if (url != value)
                {
                    url = value;
                    RaisePropertyChanged("Url");
                }
            }
        }

        public string Directory
        {
            get
            {
                return directory;
            }
            set
            {
                if (directory != value)
                {
                    directory = value;
                    RaisePropertyChanged("Directory");
                }
            }
        }

        public string XmlString
        {
            get
            {
                return xmlString;
            }
            set
            {
                if (xmlString != value)
                {
                    xmlString = value;
                    RaisePropertyChanged("XmlString");

                }
            }
        }

        public bool CanExecuteClient
        {
            get
            {
                return canExecuteClient;
            }
            set
            {
                if (canExecuteClient != value)
                {
                    canExecuteClient = value;
                }
            }
        }

        public bool CanExecute
        {
            get
            {
                return canExecute;
            }
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
            if (!String.IsNullOrEmpty(XmlString) && !String.IsNullOrEmpty(Directory))
            {
                CanExecuteClient = true;
            }
            else
            {
                CanExecuteClient = false;
            }
            if (!String.IsNullOrEmpty(XmlString) && !String.IsNullOrEmpty(Url) && !String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password))
            {
                CanExecute = true;
            }
            else
            {
                CanExecute = false;
            }
        }
    }
}
