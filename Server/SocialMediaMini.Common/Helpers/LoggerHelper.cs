using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.Helpers
{
    public class LoggerHelper
    {
        private static readonly SemaphoreSlim _logLockEX = new(1, 1);
        private static readonly SemaphoreSlim _logLockInfo = new(1, 1);

        public static async Task LogExceptionAsync(string ip, string methodName, string msgInfo, Exception ex)
        {
            await _logLockEX.WaitAsync();
            try
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string dailyFolder = Path.Combine(Utils.GetPathLogException(), currentDate);

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
                    IP         : {ip}
                    Method     : {methodName}
                    Message    : {msgInfo}
                    Exception  : {ex.Message}--{ex.StackTrace}
                    ========================================================================";
                    await sw.WriteLineAsync(content);
                    await sw.WriteLineAsync("\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _logLockEX.Release();
            }
        }




        public static async Task LogInfomationAsync(string ip, string methodName, string msgInfo)
        {
            await _logLockInfo.WaitAsync();
            try
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string dailyFolder = Path.Combine(Utils.GetPathLogInformation(), currentDate);

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
                    IP         : {ip}
                    Method     : {methodName}
                    Message    : {msgInfo}
                    ========================================================================";
                    await sw.WriteLineAsync(content);
                    await sw.WriteLineAsync("\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _logLockInfo.Release();
            }
        }

    }
}

