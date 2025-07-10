using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for ImageWindowView.xaml
    /// </summary>
    public partial class ImageWindowView : Window
    {
        public ImageWindowView(ImageSource source)
        {
            InitializeComponent();
            MainImage.Source = source;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = MainImage.Source as BitmapSource;
            if (bitmap == null)
            {
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg",
                FileName = "image.jpg",
                OverwritePrompt = true,
                AddExtension = true
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        var encoder = new JpegBitmapEncoder() { QualityLevel = 100 };
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
