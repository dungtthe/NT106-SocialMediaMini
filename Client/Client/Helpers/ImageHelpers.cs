using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.Helpers
{
    public static class ImageHelpers
    {
        public static BitmapImage Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using (var ms = new MemoryStream(imageBytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze(); // Tùy chọn để tránh lỗi cross-thread
                return image;
            }
        }

        public static string ImageToBase64(BitmapImage bitmapImage)
        {
            var encoder = new PngBitmapEncoder(); // Hoặc JpegBitmapEncoder tùy định dạng
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static string ImageSourceToBase64(ImageSource imageSource)
        {
            if (imageSource is not BitmapSource bitmapSource)
                return null;

            var encoder = new PngBitmapEncoder(); 
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var ms = new MemoryStream();
            encoder.Save(ms);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }


    }
}
