using Client.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class MainWindowViewModel:BaseViewModel
    {

        private static MainWindowViewModel ins;
        public static MainWindowViewModel GI()
        {
            if(ins == null)
            {
                ins = new MainWindowViewModel();
            }
            return ins;
        }


        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value, nameof(FullName));
        }

        private string _avatar;
        public string Avatar
        {
            get => _avatar;
            set => SetProperty(ref _avatar, value, nameof(Avatar));
        }


        public MainWindowViewModel()
        {
            FullName = UserStore.FullName;
            Avatar = UserStore.Avatar;
        }
    }
}
