using System.Windows.Controls;

namespace OneOffixx.ConnectClient.WinApp.ViewContent
{
    /// <summary>
    /// Interaction logic for SuccessView.xaml
    /// </summary>
    public partial class SuccessView : UserControl
    {
        public SuccessView(ViewModel.RequestViewModel data)
        {
            InitializeComponent();
            this.DataContext = data;
        }
    }
}
