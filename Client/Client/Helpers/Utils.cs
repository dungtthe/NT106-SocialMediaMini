using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public static class Utils
    {
        public static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static string GetPathLog()
        {
            return Path.Combine(DesktopPath, "ClientLogs");
        }
       
    }
}
