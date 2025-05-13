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

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindowView.xaml
    /// </summary>
    public partial class RegisterWindowView : Window
    {
        public RegisterWindowView()
        {
            InitializeComponent();
            MainWindow.TypePage = MainWindow.TYPE_PAGE.NONE;
        }

        private void LogInTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindowView();
            loginWindow.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            (new MessageBox.Error("test")).ShowDialog();
        }
    }
}
