using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaemonApp
{
    internal class MyHostedService : IHostedService
    {

        private readonly ILogger<MyHostedService> logger;
        private readonly IHostApplicationLifetime lifetime;
        private Timer timer;

        public MyHostedService(ILogger<MyHostedService> logger, IHostApplicationLifetime lifetime)
        {
            this.logger = logger;
            this.lifetime = lifetime;
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
            timer.Dispose();
        }

        private void OnStarting()
        {
            logger.LogInformation("Service Starting...");
            timer = new Timer(new TimerCallback(ProcessTask), null, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
        }

        private void ProcessTask(object state)
        {
            var random = new Random();
            var order = new Order
            {
                Id = random.Next(),
                Date = DateTime.Now.Date,
                Items = Enumerable.Range(0, 2).Select(sel => new OrderItem
                {
                    Id = random.Next(1, 10000000),
                    ItemName = $"Item {sel + 1}",
                    Quantity = random.Next(1, 10)
                }).ToArray()
            };
            switch (random.Next(0, 5))
            {
                case 0:
                    logger.LogDebug(new EventId(1, "OrderCreated"), "The value for the order is {@order}", order);
                    break;
                case 1:
                    logger.LogError(new EventId(4, "OrderCancelled"), new ArgumentNullException(), "There was no order total for the order {@order}", order);
                    break;
                case 2:
                default:
                    logger.LogInformation(new EventId(2, "OrderCreated"), "This log contains an order id {orderId} for the order {@order}", order.Id, order);
                    break;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service Stop initiated...");
            return Task.CompletedTask;
        }

    }

    public class Order
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public IEnumerable<OrderItem> Items { get; set; }

    }

    public class OrderItem
    {

        public int Id { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }

    }

}