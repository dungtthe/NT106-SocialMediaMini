using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ViewModels.Chats.ConversationViewModel.ItemChatRoomDetailViewModel;
using System.Windows.Data;
using System.Windows;

namespace Client.Converters
{
    public class TypeMessageToVisibilityConverter : IValueConverter
    {
        public TypeMessage TargetType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TypeMessage actualType)
                return actualType == TargetType ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
