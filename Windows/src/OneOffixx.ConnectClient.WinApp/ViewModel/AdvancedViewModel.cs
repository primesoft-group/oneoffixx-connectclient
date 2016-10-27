using OneOffixx.ConnectClient.WinApp.Helpers;
using OneOffixx.ConnectClient.WinApp.HistoryStore;
using OneOffixx.ConnectClient.WinApp.Model;
using OneOffixx.ConnectClient.WinApp.ViewContent;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OneOffixx.ConnectClient.WinApp.ViewModel
{
    public class AdvancedViewModel : INotifyPropertyChanged
    {
        private string numberOfRequests;
        private string parallelRequests;
        private AdvancedSettings advSettings;
        private RequestModel request;
        private ObservableCollection<Log> multipleRequests;
        private bool canExecuteSend = false;
        private int parallel;
        private object locker;
        private string borderColorParallel = "#FFFFFF";
        private string borderColorRequests = "#FFFFFF";
        private RequestSender sender;
        private bool close = false;

        public string BorderColorRequests
        {
            get
            {
                return borderColorRequests;
            }
            set
            {
                if (borderColorRequests != value)
                {
                    borderColorRequests = value;
                    RaisePropertyChanged("BorderColorRequests");
                }
            }
        }
        public string BorderColorParallel
        {
            get
            {
                return borderColorParallel;
            }
            set
            {
                if (borderColorParallel != value)
                {
                    borderColorParallel = value;
                    RaisePropertyChanged("BorderColorParallel");
                }
            }
        }
        public ICommand Close { get; set; }
        public ICommand Send { get; set; }
        public string NumberOfRequests
        {
            get
            {
                return numberOfRequests;
            }
            set
            {
                if (numberOfRequests != value)
                {
                    numberOfRequests = value;
                    if (numberOfRequests != "") { canExecuteSend = true; }
                    else { canExecuteSend = false; }
                    RaisePropertyChanged("NumberOfRequests");
                }
            }
        }
        public string NumberOfRequestsParallel
        {
            get
            {
                return parallelRequests;
            }
            set
            {
                if (parallelRequests != value)
                {
                    parallelRequests = value;
                    RaisePropertyChanged("NumberOfRequestsParallel");
                }
            }
        }
        public ObservableCollection<Log> MultipleRequests
        {
            get
            {
                return multipleRequests;
            }
            set
            {
                if (multipleRequests != value)
                {
                    multipleRequests = value;
                    RaisePropertyChanged("MultipleRequests");
                }
            }
        }

        public AdvancedViewModel(RequestModel request)
        {
            Send = new RelayCommand(ExecuteAdvancedRequest, param => canExecuteSend);
            Close = new RelayCommand(ExecuteClose, param => true);
            advSettings = new AdvancedSettings(this);
            this.request = request;
            locker = new object();
        }

        public void ExecuteAdvancedRequest(object obj)
        {
            canExecuteSend = false;
            int requests;
            decimal time = 0;
            MultipleRequests = new ObservableCollection<Log>();
            try
            {
                if (Int32.Parse(NumberOfRequests) > 0 && int.TryParse(NumberOfRequests, out requests) == true)
                {
                    BorderColorRequests = "#FFFFFF";
                    requests = int.Parse(NumberOfRequests);
                    if (NumberOfRequestsParallel == null || Int32.Parse(NumberOfRequestsParallel) == 0 || Int32.TryParse(NumberOfRequestsParallel, out parallel) == false)
                    {
                        parallel = 1;
                    }
                    if (parallel <= 100)
                    {
                        sender = new RequestSender(request);
                        advSettings.PbStatus.Value = 0;
                        advSettings.PbStatus.Maximum = requests;
                        BorderColorParallel = "#FFFFFF";
                        var counter = 0;
                        Parallel.For(0, parallel, async i =>
                        {
                            do
                            {
                                var index = Interlocked.Increment(ref counter) - 1;
                                if (index >= requests || close == true)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        lock (locker)
                                        {
                                            canExecuteSend = true;
                                            Send.CanExecute(canExecuteSend);
                                            advSettings.Timeused.Text = time.ToString();
                                        }
                                    });
                                    break;
                                }
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                var result = await sender.SendRequestTask();
                                sw.Stop();
                                using (var content = result.Content)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        lock (locker)
                                        {
                                            MultipleRequests.Add(new Log() { Action = "Server", ResponseEntry = new Response() { Filename = content.Headers.ContentDisposition?.FileName, StatusCode = ((int)result.StatusCode).ToString() } });
                                            advSettings.PbStatus.Value += 1;
                                            time += Math.Round((decimal)sw.ElapsedMilliseconds / 1000, 3);
                                        }
                                    });
                                }
                            }
                            while (true);
                        });
                    }
                    else
                    {
                        canExecuteSend = true;
                        BorderColorParallel = "#ffcccc";
                    }
                }
            }
            catch (OverflowException)
            {
                BorderColorRequests = "#ffcccc";
            }
        }

        public Task<HttpResponseMessage> SendRequestTask()
        {
            using (HttpClient client = new HttpClient())
            {
                var inBytes = Encoding.ASCII.GetBytes($"{request.Username}:{request.Password}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(inBytes));
                HttpContent content = new StringContent(request.XmlString);
                return client.PostAsync(request.Url, content);
            }
        }

        public void ExecuteClose(object obj)
        {
            close = true;
            ((RequestViewModel)Application.Current.MainWindow.DataContext).ExecuteClose(close);
        }

        public AdvancedSettings GetContext()
        {
            return advSettings;
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
