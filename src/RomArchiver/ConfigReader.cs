using System.Configuration;

namespace RomArchiver
{
    internal class ConfigReader
    {
        private static ConfigReader? _instance;

        internal static ConfigReader Instance
        {
            get
            {
                _instance ??= new ConfigReader();
                return _instance;
            }
        }

        internal string OfflineListCacheDirectory { get; }

        internal string SevenZipExecutablePath { get; }

        internal string RomDirectory { get; }

        internal string WorkingDirectory { get; }

        private ConfigReader()
        {
            OfflineListCacheDirectory = ConfigurationManager.AppSettings["OfflineListCacheDirectory"];
            RomDirectory = ConfigurationManager.AppSettings["RomDirectory"];
            SevenZipExecutablePath = ConfigurationManager.AppSettings["SevenZipExecutablePath"];
            WorkingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
        }
    }
}
