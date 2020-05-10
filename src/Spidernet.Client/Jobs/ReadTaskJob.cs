using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spidernet.BLL.Configs;
using Spidernet.Client.Services;
using Spidernet.Model.Models;
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
    private readonly ClientService clientService;

    public ReadTaskJob(ILogger<ReadTaskJob> logger, IOptions<SpidernetClientConfig> spidernetClientConfig, ClientService clientService) {
      _logger = logger;
      this.spidernetClientConfig = spidernetClientConfig.Value;
      this.clientService = clientService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

      var taskFactory = new ConnectionFactory();

      taskFactory.UserName = spidernetClientConfig.TaskInputMQConfig.UserName;
      taskFactory.Password = spidernetClientConfig.TaskInputMQConfig.Password;
      taskFactory.VirtualHost = spidernetClientConfig.TaskInputMQConfig.VirtualHost;
      taskFactory.HostName = spidernetClientConfig.TaskInputMQConfig.HostName;

      var resultFactory = new ConnectionFactory();

      resultFactory.UserName = spidernetClientConfig.ResultOutputMQConfig.UserName;
      resultFactory.Password = spidernetClientConfig.ResultOutputMQConfig.Password;
      resultFactory.VirtualHost = spidernetClientConfig.ResultOutputMQConfig.VirtualHost;
      resultFactory.HostName = spidernetClientConfig.ResultOutputMQConfig.HostName;

      IConnection resultConnectionConn = resultFactory.CreateConnection();


      using (var taskConnection = taskFactory.CreateConnection()) {

        using (var taskChannel = taskConnection.CreateModel()) {

          taskChannel.QueueDeclare(spidernetClientConfig.TaskInputMQConfig.Queue, false, false, false, null);

          var consumer = new EventingBasicConsumer(taskChannel);

          taskChannel.BasicConsume(spidernetClientConfig.TaskInputMQConfig.Queue, false, consumer);

          consumer.Received += async (model, ea) => {

            var messageData = Encoding.UTF8.GetString(ea.Body.ToArray());

            // 获取请求Task数据
            TaskModel taskInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskModel>(messageData);

            // 获取结果
            var result = await clientService.ExecuteTask(taskInfo);

            // 传入下一阶段
            byte[] body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(result));
            IModel resultChannel = resultConnectionConn.CreateModel();
            resultChannel.QueueDeclare(spidernetClientConfig.ResultOutputMQConfig.Queue, false, false, false, null);
            resultChannel.BasicPublish("", spidernetClientConfig.ResultOutputMQConfig.Queue, null, body); //开始传递  

            _logger.LogInformation("已接收： {0}", taskInfo.TaskId);

            // 消费该数据
            taskChannel.BasicAck(ea.DeliveryTag, false);

          };

          Console.ReadLine();
        }
      }
    }
  }
}
