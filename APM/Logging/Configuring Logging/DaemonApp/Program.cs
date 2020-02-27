using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DaemonApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            NLog.Config.ConfigurationItemFactory.Default.JsonConverter = new JsonNetSerializer();   
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal);
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception excp)
            {
                logger.Error(excp, "Unable to continue execution");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<MyHostedService>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                    logging.AddNLog();
                });
        }

    }
}
