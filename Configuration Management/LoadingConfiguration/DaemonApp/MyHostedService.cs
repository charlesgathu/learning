using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DaemonApp
{
    internal class MyHostedService : IHostedService
    {

        private readonly ILogger<MyHostedService> logger;
        private readonly IHostApplicationLifetime lifetime;
        private readonly Dictionary<string, object> keys;

        public MyHostedService(ILogger<MyHostedService> logger, IHostApplicationLifetime lifetime, Dictionary<string, object> keys)
        {
            this.logger = logger;
            this.lifetime = lifetime;
            this.keys = keys;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service Start initiated...");
            lifetime.ApplicationStarted.Register(OnStarting);
            lifetime.ApplicationStopping.Register(OnStopping);
            return Task.CompletedTask;
        }

        private void OnStopping()
        {
            logger.LogInformation("Service stopping...");
        }

        private void OnStarting()
        {
            logger.LogInformation("Service Starting...");
            var task = Task.Run(async () => {
                await ProcessTaskAsync();
            });
        }

        private Task ProcessTaskAsync()
        {
            logger.LogInformation($"variable1 is {keys["variable1"]}");
            logger.LogInformation($"variable1 is {keys["variable2"]}");
            logger.LogInformation($"variable1 is {keys["variable3"]}");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service Stop initiated...");
            return Task.CompletedTask;
        }
    }
}
