/* =============================================================================
 * Copyright (C) by Sevitec AG
 *
 * Project: OneOffixx.ConnectClient.WinApp.HistoryStore
 * 
 * =============================================================================
 * */

using OneOffixx.ConnectClient.WinApp.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OneOffixx.ConnectClient.WinApp.HistoryStore
{
    public class Log : INotifyPropertyChanged
    {
        private const string Green = "#19E866";
        private const string Red = "#FF0000";
        private const string ServerIcon = "Cloud";
        private const string ClientIcon = "Desktop";
        private const string Favorite = "Star";
        private const string NotFavourite = "StarOutline";

        private readonly ViewModel.RequestViewModel viewmodel;
        private bool isEditing;
        private string editName;
        private string name;

        public ICommand DeleteValue { get; set; }
        public ICommand LoadHistory { get; set; }
        public ICommand ChangeIsFavourite { get; set; }

        public Log(ViewModel.RequestViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
            LoadHistory = new RelayCommand(viewmodel.LoadValues, param => true);
            DeleteValue = new RelayCommand(viewmodel.ExecuteDeleteValue, param => true);
            ChangeIsFavourite = new RelayCommand(ExecuteChangeIsFavourite, param => true);
        }

        public Log()
        {
        }

        public Guid LogGuid { get; set; }
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
        public bool IsFavourite { get; set; }

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
                if(IsFavourite == false)
                {
                    return NotFavourite;
                }
                else
                {
                    return Favorite;
                }
            }
        }
        
        public void ExecuteChangeIsFavourite(object obj)
        {
            Log item = (Log)obj;
            if (item.IsFavourite)
            {
                viewmodel.Request.Log.Where(x => x.Equals(item)).FirstOrDefault().IsFavourite = false;
                viewmodel.Request.FavouriteLog.Remove(item);
                item.IsFavourite = false;
                RaisePropertyChanged("FavoriseIcon");
            }
            else
            {
                viewmodel.Request.Log.Where(x => x.Equals(item)).FirstOrDefault().IsFavourite = true;
                this.IsFavourite = true;
                viewmodel.Request.FavouriteLog.Add(item);
                viewmodel.Request.FavouriteLog = new System.Collections.ObjectModel.ObservableCollection<Log>(viewmodel.Request.FavouriteLog.OrderByDescending(x => x.RequestEntry.Date));
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
