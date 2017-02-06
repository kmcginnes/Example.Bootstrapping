using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;

namespace Example.Bootstrapping.TopShelf
{
    internal class Log4NetConfig
    {
        public static void Setup()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();

            var fileAppender = CreateFileAppender();
            hierarchy.Root.AddAppender(fileAppender);

            var consoleAppender = GetConsoleAppender();
            hierarchy.Root.AddAppender(consoleAppender);
            
            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }

        private static RollingFileAppender CreateFileAppender()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var logPath = Path.Combine(programData, "Example.Bootstapping.TopShelf", "app.log");

            var filePatternLayout = new PatternLayout
            {
                ConversionPattern = "%date %-5level [%2thread][%logger{1}]  %message%newline"
            };
            filePatternLayout.ActivateOptions();

            var roller = new RollingFileAppender
            {
                AppendToFile = true,
                File = logPath,
                Layout = filePatternLayout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "10MB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true
            };
            roller.ActivateOptions();
            return roller;
        }

        private static ColoredConsoleAppender GetConsoleAppender()
        {
            var consolePatternLayout = new PatternLayout
            {
                ConversionPattern = "%date{HH:mm:ss,fff} %-5level [%2thread][%logger{1}]  %message%newline"
            };
            consolePatternLayout.ActivateOptions();

            var console = new ColoredConsoleAppender
            {
                Layout = consolePatternLayout,
            };
            console.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Info,
                ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
            });
            console.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Debug,
                ForeColor = ColoredConsoleAppender.Colors.White,
            });
            console.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Warn,
                ForeColor = ColoredConsoleAppender.Colors.Yellow,
            });
            console.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
            });
            console.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Fatal,
                ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
            });
            console.ActivateOptions();
            return console;
        }
    }
}
