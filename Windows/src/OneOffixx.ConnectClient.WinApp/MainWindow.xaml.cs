using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OneOffixx.ConnectClient.WinApp.ViewContent;
using OneOffixx.ConnectClient.WinApp.ViewWindows;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace OneOffixx.ConnectClient.WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            AppVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string url = ((Button)e.Source).ToolTip.ToString();
            System.Diagnostics.Process.Start(url);
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ResponseWindow dial = new ResponseWindow();
            License licenseWindow = new License();
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("OneOffixx.ConnectClient.WinApp.Licenses.Licenses.txt"))
            {
                var reader = new StreamReader(stream);
                licenseWindow.Content.Text = reader.ReadToEnd();
            }
            dial.Height = 400;
            dial.Width = 700;
            dial.Content = licenseWindow;
            this.ShowMetroDialogAsync(dial);
        }
    }
}
