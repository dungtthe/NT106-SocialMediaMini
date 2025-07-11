using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Views.Notifications.Components
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRead && !isRead)
                return new SolidColorBrush(Colors.Red); // Chưa đọc
            return new SolidColorBrush(Colors.Green); // Đã đọc
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}