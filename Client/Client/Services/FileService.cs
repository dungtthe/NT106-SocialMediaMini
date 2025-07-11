using Client.Helpers;
using Client.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client.Services
{
    public static class FileService
    {
        //list image
        public static async Task<string> UploadImageAsync(List<Image> images)
        {
            var files = new List<(string fileName, byte[] content)>();

            foreach (var image in images)
            {
                if (image.Source is BitmapSource bitmap)
                {
                    var encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 100;
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));

                    using var ms = new MemoryStream();
                    encoder.Save(ms);
                    files.Add((Guid.NewGuid() + ".png", ms.ToArray()));
                }
            }

            var apiResponse = await ApiHelpers.PostFileAsync("/api/file/upload-image", files);

            if (apiResponse.StatusCode == 200)
            {
                return apiResponse.ResponseBody;
            }
            return null;
        }
    }
}
