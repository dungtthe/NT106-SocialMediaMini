using Client.Const;
using Client.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Client.Converters
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string input || string.IsNullOrWhiteSpace(input))
                return "pack://application:,,,/Resources/Images/no_img_user.png";


            if (input == "no_img_user.png" || input == "pack://application:,,,/Resources/Images/no_img_user.png" || input == "/Resources/Images/no_img_user.png")
            {
                return "pack://application:,,,/Resources/Images/no_img_user.png";
            }
            else if (string.IsNullOrEmpty(input) || input == "no_img_group.png" || input == "pack://application:,,,/Resources/Images/no_img_group.png" || input == "/Resources/Images/no_img_group.png")
            {
                return "pack://application:,,,/Resources/Images/no_img_group.png";
            }

            try
            {

                using var webClient = new WebClient();
                byte[] data = webClient.DownloadData(ConfigConst.BaseApiUrl + "/api/file/image/" + input);
                using var ms = new MemoryStream(data);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;


                //// Base64 xử lý riêng
                //if (input.StartsWith("data:image") || input.Length > 100)
                //{
                //    if (input.Contains("base64,"))
                //        input = input.Substring(input.IndexOf("base64,") + 7);

                //    return ImageHelpers.Base64ToImage(input);
                //}
                //else if (input.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                //{
                //    using var webClient = new WebClient();
                //    byte[] data = webClient.DownloadData(input);
                //    using var ms = new MemoryStream(data);

                //    var image = new BitmapImage();
                //    image.BeginInit();
                //    image.CacheOption = BitmapCacheOption.OnLoad;
                //    image.StreamSource = ms;
                //    image.EndInit();
                //    image.Freeze();
                //    return image;
                //}
                //else
                //{
                //    var image = new BitmapImage();
                //    image.BeginInit();
                //    image.UriSource = new Uri(input, UriKind.RelativeOrAbsolute);
                //    image.CacheOption = BitmapCacheOption.OnLoad;
                //    image.EndInit();
                //    image.Freeze();
                //    return image;
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Converter Error: {ex.Message}");
                return "pack://application:,,,/Resources/Images/no_img_user.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
