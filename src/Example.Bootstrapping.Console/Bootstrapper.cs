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
            var logging = new LoggingOrchestrator();
            logging.InitializeLogging("Main", banner.ToString());

            GlobalExceptionHandlers.WireUp();

            const string appId = "bootstrapping-console";
            var environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(environment, appSettings, appId);

            this.Log().Debug("Initializing the IoC container...");
            var container = InitializeContainer(appSettings);

            this.Log().Debug($"Finished bootstrapping {appId}");
            return Task.FromResult(true);
        }
        
        private IDependencyContainer InitializeContainer(AppSettings appSettings)
        {
            return (IDependencyContainer) null;
        }
    }
}