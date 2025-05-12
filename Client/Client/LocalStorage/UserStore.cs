using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.LocalStorage
{
    public static class UserStore
    {
        public static long UserIdCur = -1;
        public static string Avatar = null;
        public static string FullName = null;
        public static string Token = null;


        public static void Reset()
        {
            UserIdCur = -1;
            Avatar = null;
            FullName = null;
            Token = null;
        }
    }
}
