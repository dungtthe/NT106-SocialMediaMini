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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views.AddFriend.Pages
{
    /// <summary>
    /// Interaction logic for AddFriendPage.xaml
    /// </summary>
    public partial class AddFriendPage : Page
    {
        public AddFriendPage()
        {
            InitializeComponent();
        }

        public class UserModel
        {
            public string FullName { get; set; }
            public string Avatar { get; set; }
        }
    }
}
