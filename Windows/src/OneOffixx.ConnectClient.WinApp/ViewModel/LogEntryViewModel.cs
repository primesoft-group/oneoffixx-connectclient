using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using OneOffixx.ConnectClient.WinApp.Helpers;

namespace OneOffixx.ConnectClient.WinApp.ViewModel
{
    public class LogEntryViewModel : INotifyPropertyChanged
    {
        private const string Green = "#19E866";
        private const string Red = "#FF0000";
        private const string ServerIcon = "Cloud";
        private const string ClientIcon = "Desktop";
        private const string Favorite = "Star";
        private const string NotFavorite = "StarOutline";

        private readonly ViewModel.ShellViewModel viewmodel;
        private bool isEditing;
        private string editName;
        private string name;

        public ICommand DeleteValue { get; set; }
        public ICommand LoadHistory { get; set; }
        public ICommand ChangeIsFavorite { get; set; }

        public LogEntryViewModel(ViewModel.ShellViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
            LoadHistory = new RelayCommand(viewmodel.LoadValues, param => true);
            DeleteValue = new RelayCommand(viewmodel.ExecuteDeleteValue, param => true);
            ChangeIsFavorite = new RelayCommand(ExecuteChangeIsFavorite, param => true);
        }

        public LogEntryViewModel()
        {
        }

        public Guid Id { get; set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(name != value)
                {
                    name = value;
                    EditName = name;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public string EditName
        {
            get
            {
                return editName;
            }
            set
            {
                if(editName != value)
                {
                    editName = value;
                    RaisePropertyChanged("EditName");
                }
            }
        }
        public string Action { get; set; }
        public Request RequestEntry { get; set; }
        public Response ResponseEntry { get; set; }
        public bool IsFavorite { get; set; }

        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                RaisePropertyChanged("IsEditing");
            }
        }

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

        public string FavoriseIcon
        {
            get
            {
                if(IsFavorite == false)
                {
                    return NotFavorite;
                }
                else
                {
                    return Favorite;
                }
            }
        }
        
        public void ExecuteChangeIsFavorite(object obj)
        {
            LogEntryViewModel item = (LogEntryViewModel)obj;
            if (item.IsFavorite)
            {
                viewmodel.Request.Log.Where(x => x.Equals(item)).FirstOrDefault().IsFavorite = false;
                viewmodel.Request.FavoriteLog.Remove(item);
                item.IsFavorite = false;
                RaisePropertyChanged("FavoriseIcon");
            }
            else
            {
                viewmodel.Request.Log.Where(x => x.Equals(item)).FirstOrDefault().IsFavorite = true;
                this.IsFavorite = true;
                viewmodel.Request.FavoriteLog.Add(item);
                viewmodel.Request.FavoriteLog = new System.Collections.ObjectModel.ObservableCollection<LogEntryViewModel>(viewmodel.Request.FavoriteLog.OrderByDescending(x => x.RequestEntry.Date));
                RaisePropertyChanged("FavoriseIcon");
            }
            viewmodel.SaveHistory();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }

        }
    }
}
