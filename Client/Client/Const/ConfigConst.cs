using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Const
{
    public static class ConfigConst
    {
        public static string BaseApiUrl { get; private set; }

        static ConfigConst()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (File.Exists(configFilePath))
            {
                try
                {
                    var json = File.ReadAllText(configFilePath);

                    var config = JsonConvert.DeserializeObject<Config>(json);

                    BaseApiUrl = config?.BaseApiUrl;
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }
        }

        private class Config
        {
            public string BaseApiUrl { get; set; }
        }
    }
}
