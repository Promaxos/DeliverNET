using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeliverNET.Services
{
    internal class OrderTimeWatcher : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private Timer _timer;
        private const int timeOutTimeInSec = 60;

        public OrderTimeWatcher(
            IServiceScopeFactory scopeFactory,
            ILogger<OrderTimeWatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DeliverNETContext>();

                DateTime currentTime = DateTime.Now;
                List<Order> orders = db.Orders.Where(o => o.IsTimedOut == false).ToList();

                if (orders.Count == 0)
                    _logger.LogInformation("No active orders!!");
                else
                {
                    foreach (Order order in orders)
                    {
                        TimeSpan timeDiff = currentTime - order.Tstamp;
                        _logger.LogInformation($"Order {order.Id} - Tstamp: {order.Tstamp} - Tnow: {currentTime} - Tdiff: {timeDiff.TotalSeconds}");

                        if (timeDiff.TotalSeconds >= timeOutTimeInSec && !order.IsAccepted)
                        {
                            Order timedoutOrder = db.Orders.Find(order.Id);
                            timedoutOrder.IsTimedOut = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
