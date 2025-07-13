using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Service
{
    public static class UserServiceHelper
    {
        public static ConcurrentDictionary<string,string> PasswordResetTokenToEmailMap { get; set; } = new ConcurrentDictionary<string,string>();


        public static string GenerateLinkResetPassword()
        {
            return Guid.NewGuid().ToString();//tam thoi nhu nay da
        }


        public static string GeneratePassword()
        {
            return "123456";//tam thoi nhu nay
        }
    }
}
