using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.Helpers
{
    public static class Utils
    {
        public static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static readonly string AppName = "SocialMediaMini";
        private static string GetPathLog()
        {
            return Path.Combine(DesktopPath, AppName, "Logs");
        }
        public static string GetPathUploadImage()
        {
            return Path.Combine(DesktopPath, AppName, "Uploads", "Images");
        }
        public static string GetPathLogException()
        {
            return Path.Combine(DesktopPath, AppName, "LogException");
        }
        public static string GetPathLogInformation()
        {
            return Path.Combine(DesktopPath, GetPathLog(), "LogInformation");
        }

        public static string RandomPassword()
        {
            Random random = new Random();
            int length = random.Next(6, 11);
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }
    }
}
