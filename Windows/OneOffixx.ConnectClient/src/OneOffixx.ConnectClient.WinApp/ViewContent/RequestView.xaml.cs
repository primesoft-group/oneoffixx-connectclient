using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OneOffixx.ConnectClient.WinApp.ViewContent
{
    /// <summary>
    /// Interaction logic for RequestView.xaml
    /// </summary>
    public partial class RequestView : UserControl
    {
        public RequestView()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.RequestViewModel();
        }

        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            ((ViewModel.RequestViewModel)this.DataContext).PreviewDrop(e);
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = ((ViewModel.RequestViewModel)this.DataContext).PreviewDragOver(e);
        }
    }
}
