using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example.Bootstrapping.Console
{
    public class Bootstrapper
    {
        public Task Start(string[] commandLineArgs)
        {
            var banner = new StringBuilder();
            banner.AppendLine(@" ______               __          __                                     ");
            banner.AppendLine(@"|   __ \.-----.-----.|  |_.-----.|  |_.----.---.-.-----.-----.-----.----.");
            banner.AppendLine(@"|   __ <|  _  |  _  ||   _|__ --||   _|   _|  _  |  _  |  _  |  -__|   _|");
            banner.AppendLine(@"|______/|_____|_____||____|_____||____|__| |___._|   __|   __|_____|__|  ");
            banner.AppendLine(@"                                                 |__|  |__|              ");
            banner.AppendLine(@"    ");
            var logging = new LoggingOrchestrator("Main", banner.ToString());
            logging.InitializeLogging();
            this.Log().Debug("Logging initialized");

            this.Log().Debug("Wiring up global exception handlers...");
            GlobalExceptionHandlers.WireUp();

            this.Log().Debug("Gathering system information for AppContext...");
            var appContext = AppContextService.GatherAppContext("bootstrapping-console", "Example Bootstrapping Console App", Assembly.GetExecutingAssembly());
            logging.LogUsefulInformation(appContext);

            this.Log().Debug("Parsing command line and app settings...");
            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);

            ConfigurationParser.LogSettings(appSettings);

            this.Log().Debug("Initializing the IoC container...");
            var container = InitializeContainer(appSettings);

            this.Log().Debug($"Finished bootstrapping {appContext.AppId}");
            return Task.FromResult(true);
        }
        
        private IDependencyContainer InitializeContainer(AppSettings appSettings)
        {
            return (IDependencyContainer) null;
        }
    }
}