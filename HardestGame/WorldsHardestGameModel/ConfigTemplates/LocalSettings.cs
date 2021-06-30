using System.Configuration;
using System.Collections.Specialized;

namespace WorldsHardestGameModel.ConfigTemplates
{
    public class LocalSettings : ILocalSettings
    {
        private readonly NameValueCollection appSettings;

        public LocalSettings()
        {
            appSettings = ConfigurationManager.AppSettings;
        }

        public string LevelDir => appSettings["LevelDir"];

        /// <summary>
        /// Number of columns in the map
        /// </summary>
        public int MapWidth => int.Parse(appSettings["MapWidth"]);


        /// <summary>
        /// Number of rows in the map
        /// </summary>
        public int MapHeight => int.Parse(appSettings["MapHeight"]);
    }
}
