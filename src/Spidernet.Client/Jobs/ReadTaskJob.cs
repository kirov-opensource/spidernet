using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spidernet.BLL.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spidernet.Client.Jobs {
  public class ReadTaskJob : BackgroundService {
    private readonly ILogger<ReadTaskJob> _logger;
    private readonly SpidernetClientConfig spidernetClientConfig;

    public ReadTaskJob(ILogger<ReadTaskJob> logger, IOptions<SpidernetClientConfig> spidernetClientConfig) {
      _logger = logger;
      this.spidernetClientConfig = spidernetClientConfig.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

      var factory = new ConnectionFactory();

      factory.UserName = spidernetClientConfig.MQConfig.UserName;
      factory.Password = spidernetClientConfig.MQConfig.Password;
      factory.VirtualHost = spidernetClientConfig.MQConfig.VirtualHost;
      factory.HostName = spidernetClientConfig.MQConfig.HostName;

      using (var connection = factory.CreateConnection()) {
        using (var channel = connection.CreateModel()) {

          channel.QueueDeclare(spidernetClientConfig.MQConfig.Queue, false, false, false, null);

          var consumer = new EventingBasicConsumer(channel);
          channel.BasicConsume(spidernetClientConfig.MQConfig.Queue, false, consumer);
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
    }
  }
}
