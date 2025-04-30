using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.Helpers
{
    public class LoggerHelper
    {
        private static readonly SemaphoreSlim _logLock = new(1, 1);

        public static async Task LogMsgAsync(string methodName, string msgInfo, Exception ex)
        {
            await _logLock.WaitAsync();
            try
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string dailyFolder = Path.Combine(Utils.GetPathLog(), currentDate);

                if (!Directory.Exists(dailyFolder))
                {
                    Directory.CreateDirectory(dailyFolder);
                }

                string path = Path.Combine(dailyFolder, "log.txt");

                using (FileStream fStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                using (StreamWriter sw = new StreamWriter(fStream))
                {
                    string content = @$"
================== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==================
Method     : {methodName}
Message    : {msgInfo}
Exception  : {ex.Message}
========================================================================";
                    await sw.WriteLineAsync(content);
                    await sw.WriteLineAsync("\n");
                }
            }
            catch
            {
            }
            finally
            {
                _logLock.Release();
            }
        }

    }

}
