using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace OneOffixx.ConnectClient.WinApp.Views
{
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : UserControl
    {
        public AdvancedSettings(ViewModel.AdvancedViewModel dataContext)
        {
            this.DataContext = dataContext;
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
