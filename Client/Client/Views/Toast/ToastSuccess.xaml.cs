using Client.ViewModels;
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
using System.Windows.Shapes;

namespace Client.Views.Toast
{
    /// <summary>
    /// Interaction logic for ToastSuccess.xaml
    /// </summary>
    public partial class ToastSuccess : Window
    {
        public ToastSuccess(string content = "")
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            ToastMessage.Text = string.IsNullOrWhiteSpace(content)
                ? "Đã xảy ra lỗi!" : content;

            Loaded += (s, e) =>
            {
                //bottom-right
                //this.Left = SystemParameters.WorkArea.Right - this.ActualWidth - 20;
                //this.Top = SystemParameters.WorkArea.Bottom - this.ActualHeight - 20;

                //top-center
                this.Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - this.ActualWidth) / 2;
                this.Top = SystemParameters.WorkArea.Top + 20;
            };
        }

        private bool HasClosed = false;
        public void CloseToast()
        {
            if (HasClosed)
            {
                return;
            }
            HasClosed = true;
            try
            {
                ToastManager.CloseToastCur();
                this.Close();
            }
            catch { }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseToast();
        }
    }
}
