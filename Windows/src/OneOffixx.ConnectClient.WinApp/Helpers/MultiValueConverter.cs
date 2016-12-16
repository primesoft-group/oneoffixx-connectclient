using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OneOffixx.ConnectClient.WinApp.ViewModel;

namespace OneOffixx.ConnectClient.WinApp.Helpers
{
    class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = new Dictionary<LogEntryViewModel, string>();
            dictionary.Add((LogEntryViewModel)values[0], values[1].ToString());
            return dictionary;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
