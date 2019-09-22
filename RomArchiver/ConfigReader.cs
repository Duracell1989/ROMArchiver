using System.Configuration;

namespace RomLister
{
    internal class ConfigReader
    {
        private static ConfigReader _instance;

        internal static ConfigReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigReader();
                }
                return _instance;
            }
        }

        internal string OfflineListCacheDirectory { get; } = string.Empty;

        internal string SevenZipExecutablePath { get; }

        internal string RomDirectory { get; } = string.Empty;

        internal string WorkingDirectory { get; }

        private ConfigReader()
        {
            OfflineListCacheDirectory = ConfigurationManager.AppSettings["OfflineListCacheDirectory"];
            RomDirectory = ConfigurationManager.AppSettings["RomDirectory"];
            SevenZipExecutablePath = ConfigurationManager.AppSettings["SevenZipExecutablePath"];
            WorkingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
        }

        public bool VerifyConfig()
        {
            return true;
        }
    }
}
