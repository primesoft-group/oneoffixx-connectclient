using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OneOffixx.ConnectClient.WinApp.Helpers
{
    
    /// <summary>
    /// Converts a Boolean into a Visibility.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If set to True, conversion is reversed: True will become Collapsed.
        /// </summary>
        public bool IsReversed { get; set; }

        public object Convert(object value, Type typeName, object parameter, CultureInfo language)
        {
            var val = System.Convert.ToBoolean(value);
            if (this.IsReversed)
            {
                val = !val;
            }

            if (val)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type typeName, object parameter, CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }
}
