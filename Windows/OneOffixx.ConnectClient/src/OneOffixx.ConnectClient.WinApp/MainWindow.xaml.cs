using MahApps.Metro.Controls;
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
    }
}
