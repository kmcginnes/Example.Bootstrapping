using System;
using System.Collections.Specialized;
using System.IO;
using Example.Bootstrapping.Logging;

namespace Example.Bootstrapping.TopShelf
{
    public static class ConfigurationParser
    {
        private static readonly ILog Logger = typeof(ConfigurationParser).Name.Log();

        public static AppSettings Parse(string[] commandLineArgs, NameValueCollection appSettings)
        {
            Logger.Debug("Parsing command line and app settings...");
            var dataDirectory = GetDataDirectory(appSettings);
            var defaultVolume = GetDefaultVolume(appSettings);

            var settings = new AppSettings
            {
                DataDirectory = dataDirectory,
                DefaultVolume = defaultVolume,
            };
            
            return settings;
        }

        public static string GetDataDirectory(NameValueCollection appSettings)
        {
            var defaultPath = @"C:\Temp\";
            var configuredPath = appSettings["DataDirectory"];

            if (String.IsNullOrWhiteSpace(configuredPath))
            {
                Logger.Info($"AppSettings.DataDirectory was not defined. Using the default value {defaultPath}");
                return defaultPath;
            }

            if (!configuredPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                configuredPath = configuredPath + Path.DirectorySeparatorChar;
            }

            return configuredPath;
        }

        public static int GetDefaultVolume(NameValueCollection appSettings)
        {
            const int defaultVolume = 100;
            const int maxVolume = 200;
            const int minVolume = 0;
            var configuredVolume = appSettings["DefaultPlaybackVolume"];

            if (String.IsNullOrWhiteSpace(configuredVolume))
            {
                Logger.Info($"AppSettings.DefaultPlaybackVolume was not defined. Using the default value {defaultVolume}");
                return defaultVolume;
            }

            int converted;

            if (!Int32.TryParse(configuredVolume, out converted))
            {
                Logger.Info($"AppSettings.DefaultPlaybackVolume was not an integer value ({configuredVolume}). Using the default value {defaultVolume}");
                return defaultVolume;
            }

            if (converted > maxVolume)
            {
                Logger.Info($"AppSettings.DefaultPlaybackVolume was higher than the max value ({configuredVolume}). Using the max value {maxVolume}");
                return maxVolume;
            }

            if (converted < minVolume)
            {
                Logger.Info($"AppSettings.DefaultPlaybackVolume was lower than the min value ({configuredVolume}). Using the min value {minVolume}");
                return minVolume;
            }

            return converted;
        }
    }
}