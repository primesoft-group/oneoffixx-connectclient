using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OneOffixx.ConnectClient.WinApp.Helpers;
using OneOffixx.ConnectClient.WinApp.HistoryStore;
using OneOffixx.ConnectClient.WinApp.Model;
using OneOffixx.ConnectClient.WinApp.ViewContent;
using OneOffixx.ConnectClient.WinApp.ViewWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace OneOffixx.ConnectClient.WinApp.ViewModel
{
    public class RequestViewModel : INotifyPropertyChanged
    {
        public ICommand Connect { get; set; }
        public ICommand Send { get; set; }
        public ICommand Save { get; set; }
        public ICommand Open { get; set; }
        public ICommand Browse { get; set; }
        public ICommand ShowError { get; set; }
        public ICommand Clear { get; set; }
        public ICommand Checkbox { get; set; }
        public ICommand Close { get; set; }
        public ICommand OpenFlyout { get; set; }
        public ICommand ClearInput { get; set; }
        public ICommand LoadHistory { get; set; }
        public ICommand AdvancedRequest { get; set; }
        public ICommand OpenAdvSettings { get; set; }
        public ICommand OpenUrl { get; set; }
        public ICommand Validate { get; set; }

        public RequestModel Request
        {
            get;
            set;
        }

        private AdvancedViewModel advView;
        private bool isFlyoutOpen = false;
        private Visibility pwVisibility = Visibility.Hidden;
        private byte[] byteResult;
        private string filename;
        private History log;
        private bool isErrorAppeared = false;
        private ResponseWindow dial;
        private Log selectedLogItem;
        private bool validation = true;
        private string validationText;
        private readonly string historyFileName = "History.xml";

        private string GetHistorySavePath()
        {
            return Environment.ExpandEnvironmentVariables("%AppData%\\OneOffixx.ConnectClient\\");
        }

        public RequestViewModel()
        {
            log = new History();
            log.Logs = new List<Log>();
            LoadHistoryFromFile();
            Application.Current.MainWindow.DataContext = this;
            Validate = new RelayCommand(ExecuteValidation, param => true);
            Send = new RelayCommand(SendRequest, param => Request.CanExecute);
            Save = new RelayCommand(SaveFile, param => true);
            Open = new RelayCommand(OpenFile, param => true);
            Browse = new RelayCommand(BrowseExplorer, param => true);
            Connect = new RelayCommand(ClientConnect, param => Request.CanExecuteClient);
            ShowError = new RelayCommand(ShowErrorMessage, param => isErrorAppeared);
            Clear = new RelayCommand(ClearHistory, param => true);
            Checkbox = new RelayCommand(CheckBoxChanged, param => true);
            Close = new RelayCommand(ExecuteClose, param => true);
            OpenFlyout = new RelayCommand(ExecuteOpenFlyout, param => true);
            ClearInput = new RelayCommand(ExecuteClearInput, param => true);
            LoadHistory = new RelayCommand(LoadValues, param => true);
            OpenAdvSettings = new RelayCommand(ExecuteOpenAdvSettings, param => Request.CanExecute);
        }

        private void ExecuteClearInput(object obj)
        {
            Request.Directory = "";
            Request.Url = "";
            Request.Username = "";
            Request.Password = "";
            Request.XmlString = "";
        }

        public void ExecuteOpenFlyout(object obj)
        {
            IsFlyoutOpen = true;
        }

        public Visibility PwVisibility
        {
            get
            {
                return pwVisibility;
            }
            set
            {
                if (pwVisibility != value)
                {
                    pwVisibility = value;
                    RaisePropertyChanged("PwVisibility");
                }
            }
        }

        public Log SelectedLogItem
        {
            get
            {
                return selectedLogItem;
            }
            set
            {
                if (SelectedLogItem != value)
                {
                    selectedLogItem = value;
                    RaisePropertyChanged("SelectedLogItem");
                }
            }
        }

        public void CheckBoxChanged(object obj)
        {
            if ((bool)obj)
            {
                PwVisibility = Visibility.Visible;
            }
            else
            {
                PwVisibility = Visibility.Hidden;
            }
        }

        public void ShowErrorMessage(object obj)
        {
            MessageBox.Show($"Error Occured:\n {Request.Error}");
        }

        public async void ExecuteClose(object obj)
        {
            if (obj == null && advView != null)
            {
                advView.ExecuteClose(obj);
                advView = null;
            }
            var openDial = await ((MetroWindow)Application.Current.MainWindow).GetCurrentDialogAsync<BaseMetroDialog>();
            if(openDial != null)
            {
                if (dial?.Visibility == Visibility.Visible)
                {
                    if (advView != null)
                    {
                        advView = null;
                    }
                    dial.Visibility = Visibility.Hidden;
                }
                await ((MetroWindow)Application.Current.MainWindow).HideMetroDialogAsync(openDial);
            }
        }

        public async void ExecuteOpenAdvSettings(object obj)
        {
            advView = new AdvancedViewModel(Request);
            dial = new ResponseWindow();
            dial.Height = 400;
            dial.Width = 400;
            dial.Content = advView.GetContext();
            await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
        }

        public async void ClientConnect(object obj)
        {
            Log values = new Log(this);
            values.Action = "Client";
            values.RequestEntry = new Request() { Date = System.DateTime.Now, Uri = Request.Directory, Content = Request.XmlString };
            values.ResponseEntry = new Response();
            SaveHistory();
            Request.CanExecuteClient = false;
            filename = Path.GetTempPath() + Guid.NewGuid().ToString() + ".oocx";
            try
            {
                XDocument xdoc = XDocument.Parse(Request.XmlString);
                xdoc.Save(filename);
                try
                {
                    if (File.Exists(Request.Directory))
                    {
                        var command = Process.Start($"{Request.Directory}", $"/connector \"{filename}\"");
                        values.ResponseEntry.StatusCode = "200";
                    }
                    else
                    {
                        FailView fail = new FailView(this);
                        dial = new ResponseWindow();
                        fail.ServerStatus.Text = "failed";
                        values.ResponseEntry = new Response() { StatusCode = "File not found" };
                        fail.Details.Text = "Since we could not find the file pleas check your input directory and confirm if there has crept in an error. ";
                        dial.Content = fail;
                        dial.Height = 300;
                        dial.Width = 300;
                        await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                    }
                }
                catch (Exception ex)
                {
                    FailView fail = new FailView(this);
                    dial = new ResponseWindow();
                    fail.ServerStatus.Text = "Uri not found";
                    values.ResponseEntry = new Response() { StatusCode = "Unexpected Error" };
                    fail.Details.Text = "There has been an unexpected Error while trying to execute the Connector. Error: " + ex.Message;
                    dial.Content = fail;
                    dial.Height = 300;
                    dial.Width = 300;
                    await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                }
            }
            catch (System.Xml.XmlException)
            {
                FailView fail = new FailView(this);
                dial = new ResponseWindow();
                fail.ServerStatus.Text = "Uri not found";
                values.ResponseEntry = new Response() { StatusCode = "Unexpected Error" };
                fail.Details.Text = "An unexpected Error has occured while while trying to create the temp .oocx file. Please check the Xml syntax of your content input.";
                dial.Content = fail;
                dial.Height = 300;
                dial.Width = 300;
                await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
            }
            log.Logs.Add(values);
            Request.Log = new ObservableCollection<Log>(log.Logs.OrderByDescending(x => x.RequestEntry.Date));
            Request.CanExecuteClient = true;
        }

        public void BrowseExplorer(object obj)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "EXE files (*.exe)|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Request.Directory = ofd.FileName;
            }
        }

        public void ClearHistory(object obj)
        {
            log.Logs.Clear();
            Request.Log = new ObservableCollection<Log>(log.Logs);
            if (File.Exists(Path.Combine(GetHistorySavePath() + historyFileName)))
            {
                File.Delete(Path.Combine(GetHistorySavePath() + historyFileName));
            }
        }

        public void LoadValues(object value)
        {
            if (value != null)
            {
                Log oldRequest = (Log)value;
                if (oldRequest.Action == "Server")
                {
                    Request.SelectedIndex = 0;
                    Request.Url = oldRequest.RequestEntry.Uri;
                    Request.XmlString = oldRequest.RequestEntry.Content;
                    Request.Username = oldRequest.RequestEntry.Username;
                    Request.Password = oldRequest.RequestEntry.Password;
                }
                else if (oldRequest.Action == "Client")
                {
                    Request.SelectedIndex = 1;
                    Request.Directory = oldRequest.RequestEntry.Uri;
                    Request.XmlString = oldRequest.RequestEntry.Content;
                }
                SelectedLogItem = oldRequest;
            }
        }

        public void ExecuteDeleteValue(object obj)
        {
            Log itemToDelete = (Log)obj;
            bool check = log.Logs.Remove(itemToDelete);
            Request.Log.Remove(itemToDelete);
            SaveHistory();
        }

        public void SaveFile(object obj)
        {
            string path = null;
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.FileName = filename;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = ofd.FileName;
            }
            if (path != null)
            {
                try
                {
                    FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteResult, 0, byteResult.Length);
                    fileStream.Close();
                    ((MetroWindow)Application.Current.MainWindow).HideMetroDialogAsync(dial);
                }
                catch (Exception e)
                {
                    ((MetroWindow)Application.Current.MainWindow).HideMetroDialogAsync(dial);
                    FailView fail = new FailView(this);
                    dial = new ResponseWindow();
                    fail.ServerStatus.Text = "File Save Error";
                    fail.Details.Text = "There has been an Error while reading the byte stream. \r Exception: " + e.Message + "";
                    dial.Content = fail;
                    dial.Height = 300;
                    dial.Width = 300;
                    ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                }
            }
        }

        public void LoadHistoryFromFile()
        {
            string filepath = Path.Combine(GetHistorySavePath() + historyFileName);
            if (File.Exists(filepath))
            {
                try
                {
                    XDocument xdoc = XDocument.Load(filepath);
                    string xml = xdoc.ToString();
                    var elements = XmlSerializer.Deserialize<HistoryEntry>(xml);
                    foreach (var item in elements.Logs)
                    {
                        log.Logs.Add(new Log(this)
                        {
                            Action = item.Action,
                            RequestEntry = new Request() { Uri = item.RequestEntry.Uri, Username = item.RequestEntry.Username, Password = item.RequestEntry.Password, Content = item.RequestEntry.Content, Date = item.RequestEntry.Date },
                            ResponseEntry = new Response() { StatusCode = item.ResponseEntry.StatusCode, Filename = item.ResponseEntry.Filename, TimeUsed = item.ResponseEntry.TimeUsed }
                        });
                    }
                }
                catch (Exception)
                {
                    string newFilepath = GetHistorySavePath() + Guid.NewGuid().ToString() + "\\" + historyFileName;
                    System.IO.File.Move("filepath", newFilepath);
                    FailView fail = new FailView(this);
                    dial = new ResponseWindow();
                    fail.ServerStatus.Text = "History Error";
                    fail.Details.Text = "An Error occured while trying to read your History. It seems that the xml structure was changed or is from an older version. Your History file was renamed to: " + newFilepath;
                    dial.Content = fail;
                    dial.Height = 300;
                    dial.Width = 300;
                    ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                }

                RequestModel request = new RequestModel();
                if (log.Logs[0].Action == "Server")
                {
                    request.WarningVisibility = Visibility.Hidden;
                    request.Url = log.Logs[0].RequestEntry.Uri;
                    request.Username = log.Logs[0].RequestEntry.Username;
                    request.Password = log.Logs[0].RequestEntry.Password;
                    request.XmlString = log.Logs[0].RequestEntry.Content;
                    Request = request;
                }
                else if (log.Logs[0].Action == "Client")
                {
                    request.WarningVisibility = Visibility.Hidden;
                    request.Directory = log.Logs[0].RequestEntry.Uri;
                    request.XmlString = log.Logs[0].RequestEntry.Content;
                    Request = request;
                    Request.SelectedIndex = 1;
                }
                SelectedLogItem = log.Logs[0];
                Request.Log = new ObservableCollection<Log>(log.Logs);
            }
            else
            {
                RequestModel request = new RequestModel() { WarningVisibility = Visibility.Hidden };
                Request = request;
            }
        }

        public void SaveHistory()
        {
            string path = GetHistorySavePath();
            HistoryEntry history = new HistoryEntry();
            history.Logs = new List<LogEntrys>();
            foreach (var item in Request.Log)
            {
                history.Logs.Add(new LogEntrys()
                {
                    Action = item.Action,
                    RequestEntry = new RequestEntry() { Uri = item.RequestEntry.Uri, Username = item.RequestEntry?.Username, Password = item.RequestEntry?.Password, Content = item.RequestEntry.Content, Date = item.RequestEntry.Date },
                    ResponseEntry = new ResponseEntry() { StatusCode = item.ResponseEntry?.StatusCode, Filename = item.ResponseEntry?.Filename, TimeUsed = item.ResponseEntry?.TimeUsed }
                });
            }
            var xmlString = XmlSerializer.Serialize<HistoryEntry>(history, new XmlWriterSettings { Encoding = new UTF8Encoding(false) });
            XDocument xdoc = XDocument.Parse(xmlString);
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                    xdoc.Save(path + historyFileName);
                }
                catch (Exception)
                {
                    Request.WarningMessage = "The directory which you stated in the Config file could not be created. Pleas check the value Attribute in the Config file!";
                    Request.WarningVisibility = Visibility.Visible;
                }
            }
            else
            {

                xdoc.Save(path + historyFileName);
            }
        }

        public void OpenFile(object obj)
        {
            try
            {
                string tempPath = Path.GetTempFileName() + filename;
                FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
                fileStream.Write(byteResult, 0, byteResult.Length);
                fileStream.Close();
                ((MetroWindow)Application.Current.MainWindow).HideMetroDialogAsync(dial);
                var process = Process.Start(tempPath);
            }
            catch (Exception e)
            {
                ((MetroWindow)Application.Current.MainWindow).HideMetroDialogAsync(dial);
                FailView fail = new FailView(this);
                dial = new ResponseWindow();
                fail.ServerStatus.Text = "200";
                fail.Details.Text = "There has been an Error while reading the byte stream. \r Exception: " + e.Message + "";
                dial.Content = fail;
                dial.Height = 300;
                dial.Width = 300;
                ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
            }
        }

        public async void SendRequest(object obj)
        {
            double length;
            isErrorAppeared = false;
            Request.CanExecute = false;
            Log values = new Log(this);
            values.Action = "Server";
            values.RequestEntry = new Request() { Uri = Request.Url, Content = Request.XmlString, Username = Request.Username, Password = Request.Password, Date = System.DateTime.Now };
            values.ResponseEntry = new Response();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var inBytes = Encoding.ASCII.GetBytes($"{Request.Username}:{Request.Password}");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(inBytes));
                    HttpContent content = new StringContent(Request.XmlString);
                    Stopwatch sw = Stopwatch.StartNew();
                    var result = await client.PostAsync(Request.Url, content);
                    sw.Stop();
                    values.ResponseEntry.TimeUsed = Math.Round((decimal)sw.ElapsedMilliseconds / 1000, 3).ToString();
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        using (content = result.Content)
                        {
                            length = (double)content.Headers.ContentLength;
                            filename = content.Headers.ContentDisposition.FileName;
                            filename = filename.Replace("\"", "");
                            byteResult = await content.ReadAsByteArrayAsync();
                        }
                        values.ResponseEntry.StatusCode = ((int)result.StatusCode).ToString();
                        SuccessView success = new SuccessView(this);
                        dial = new ResponseWindow();
                        length = Math.Round((length / 1048576), 2);
                        success.Size.Text = length.ToString() + " MB";
                        success.Filename.Text = filename;
                        success.Time.Text = values.ResponseEntry.TimeUsed + " Seconds";
                        dial.Content = success;
                        dial.Height = 300;
                        dial.Width = 300;
                        await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                    }
                    else
                    {
                        FailView fail = new FailView(this);
                        dial = new ResponseWindow();
                        values.ResponseEntry.StatusCode = ((int)result.StatusCode).ToString();
                        fail.ServerStatus.Text = result.StatusCode.ToString() + " " + (int)result.StatusCode;
                        fail.Details.Text = result.ReasonPhrase;
                        dial.Content = fail;
                        dial.Height = 300;
                        dial.Width = 300;
                        await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                    }

                }
                catch (Exception)
                {
                    FailView fail = new FailView(this);
                    dial = new ResponseWindow();
                    fail.ServerStatus.Text = "Uri not found";
                    values.ResponseEntry = new Response() { StatusCode = "Uri not found" };
                    fail.Details.Text = "Your Server input may be wrong as we could not find the Uri you requested.";
                    dial.Content = fail;
                    dial.Height = 300;
                    dial.Width = 300;
                    await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                }
                finally
                {
                    values.ResponseEntry.Filename = filename;
                    Request.CanExecute = true;
                    log.Logs.Add(values);
                    Request.Log = new ObservableCollection<Log>(log.Logs.OrderByDescending(x => x.RequestEntry.Date));
                    SaveHistory();
                }
            }
        }

        public async void ExecuteValidation(object obj)
        {
            dial = new ResponseWindow();
            XmlSchemaSet xmlSchema = new XmlSchemaSet();
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("OneOffixx.ConnectClient.WinApp.XSD.OneOffixxValidation.xsd"))
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    var xmlString = reader.ReadToEnd();
                    xmlSchema.Add("", XmlReader.Create(new StringReader(xmlString)));
                }
            }
            try
            {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(Request.XmlString);
                xdoc.Schemas = xmlSchema;
                ValidationEventHandler eventHandler = new ValidationEventHandler(XmlSettingsValidationEventHandler);
                xdoc.Validate(eventHandler);
                if (validation)
                {
                    SuccessView success = new SuccessView(this);
                    success.tb.Visibility = Visibility.Hidden;
                    success.tb1.Visibility = Visibility.Hidden;
                    success.tb2.Visibility = Visibility.Hidden;
                    success.tb3.Visibility = Visibility.Hidden;
                    success.Save.Visibility = Visibility.Hidden;
                    success.Open.Visibility = Visibility.Hidden;
                    success.Time.Visibility = Visibility.Hidden;
                    success.Size.Visibility = Visibility.Hidden;
                    success.Filename.Visibility = Visibility.Hidden;
                    dial.Content = success;
                    dial.Height = 300;
                    dial.Width = 300;
                    await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                }
                else
                {
                    FailView fail = new FailView(this);
                    fail.ServerStatus.Text = "The XML is not valid.";
                    fail.Details.Text = validationText;
                    dial.Content = fail;
                    dial.Height = 300;
                    dial.Width = 300;
                    await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                    validation = true;
                }
            }
            catch (XmlException e)
            {
                FailView fail = new FailView(this);
                fail.ServerStatus.Text = "Xml Parse Exception";
                fail.Details.Text = "The XML contains an error. Make sure it is well formed and validate again.";
                dial.Content = fail;
                dial.Height = 300;
                dial.Width = 300;
                await ((MetroWindow)Application.Current.MainWindow).ShowMetroDialogAsync(dial);
                validation = true;
            }
        }

        private void XmlSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {

            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                validationText = e.Message;
            }
            validation = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }

        }

        public DragDropEffects PreviewDragOver(DragEventArgs drgevent)
        {
            drgevent.Handled = true;
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])drgevent.Data.GetData(DataFormats.FileDrop);
                if (files.Count() == 1)
                {
                    if (System.IO.Path.GetExtension(files[0]) == ".xml" || System.IO.Path.GetExtension(files[0]) == ".txt" || System.IO.Path.GetExtension(files[0]) == ".oocx" || System.IO.Path.GetExtension(files[0]) == ".oock")
                        return DragDropEffects.Move;
                    else
                        return DragDropEffects.None;
                }
                else
                    return DragDropEffects.None;

            }
            else
            {
                return DragDropEffects.None;
            }
        }

        public void PreviewDrop(DragEventArgs drgevent)
        {
            string file = ((string[])drgevent.Data.GetData(DataFormats.FileDrop))[0];
            Request.XmlString = File.ReadAllText(file);
        }

        public bool IsFlyoutOpen
        {
            get
            {
                return isFlyoutOpen;
            }
            set
            {
                if (isFlyoutOpen != value)
                {
                    isFlyoutOpen = value;
                    RaisePropertyChanged("IsFlyoutOpen");
                }
            }
        }
    }
}
