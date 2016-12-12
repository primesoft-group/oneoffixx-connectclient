using System.Windows.Controls;

namespace OneOffixx.ConnectClient.WinApp.Views
{
    /// <summary>
    /// Interaction logic for FailView.xaml
    /// </summary>
    public partial class FailView : UserControl
    {
        public FailView(ViewModel.RequestViewModel data)
        {
            InitializeComponent();
            this.DataContext = data;
        }
        
    }
}
