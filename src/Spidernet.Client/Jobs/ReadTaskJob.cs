using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spidernet.Client.Jobs {
  public class ReadTaskJob : BackgroundService {
    private readonly ILogger<ReadTaskJob> _logger;

    public ReadTaskJob(ILogger<ReadTaskJob> logger) {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

      var factory = new ConnectionFactory();
      factory.UserName = "user";
      factory.Password = "test@test!";
      factory.VirtualHost = "/";
      factory.HostName = "mq.kirov-opensource.com";

      using (var connection = factory.CreateConnection()) {
        using (var channel = connection.CreateModel()) {

          channel.QueueDeclare("hello", false, false, false, null);

          var consumer = new EventingBasicConsumer(channel);
          channel.BasicConsume("hello", false, consumer);
          consumer.Received += (model, ea) => {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _logger.LogInformation("已接收： {0}", message);
            channel.BasicAck(ea.DeliveryTag, false);
          };
          Console.ReadLine();
        }
      }

      //_logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
      //while (!stoppingToken.IsCancellationRequested) {
      //  _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      //  await Task.Delay(1000, stoppingToken);
      //}
    }
  }
}
