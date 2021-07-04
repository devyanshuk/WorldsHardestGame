using System.Configuration;
using System.Collections.Specialized;


namespace WorldsHardestGameView.ConfigTempates
{
    public static class FilePaths
    {
        private static NameValueCollection appSettings = ConfigurationManager.AppSettings;


        public static string BackgroundMusicPath => appSettings["BackgroundMusicPath"];

        public static string CoinImagePath => appSettings["CoinImagePath"];

    }
}
