using System.Configuration;
using System.Collections.Specialized;


namespace WorldsHardestGameView.ConfigTempates
{
    public static class FilePaths
    {
        private static NameValueCollection appSettings = ConfigurationManager.AppSettings;


        public static string BackgroundMusicPath => appSettings[nameof(BackgroundMusicPath)];

        public static string CoinImagePath => appSettings[nameof(CoinImagePath)];

        public static string LevelMessageJson => appSettings[nameof(LevelMessageJson)];

    }
}
