using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OneOffixx.ConnectClient.WinApp.ViewModel;

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
        private ObservableCollection<LogEntryViewModel> log;
        private ObservableCollection<LogEntryViewModel> _favoriteLog;
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
                RaisePropertyChanged(nameof(Error));
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
                    RaisePropertyChanged(nameof(SelectedIndex));
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
                    RaisePropertyChanged(nameof(Username));
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
                    RaisePropertyChanged(nameof(WarningVisibility));
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
                    RaisePropertyChanged(nameof(WarningMessage));
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
                    RaisePropertyChanged(nameof(Password));
                }
            }
        }

        public ObservableCollection<LogEntryViewModel> Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
                FavoriteLog = new ObservableCollection<LogEntryViewModel>(log.Where(x => x.IsFavorite));
                RaisePropertyChanged(nameof(Log));
            }
        }

        public ObservableCollection<LogEntryViewModel> FavoriteLog
        {
            get
            {
                return _favoriteLog;
            }
            set
            {
                _favoriteLog = value;
                RaisePropertyChanged(nameof(FavoriteLog));
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
                    RaisePropertyChanged(nameof(Url));
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
                    RaisePropertyChanged(nameof(Directory));
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
                    RaisePropertyChanged(nameof(XmlString));
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
