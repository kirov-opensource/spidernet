using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using Spidernet.Model.Enums;
using Spidernet.Model.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Spidernet.Client.Tests {
  public class SpidernetTests {

    [Fact]
    public async void JSONSchemaValidation() {
      var a = new JSchemaGenerator();
      var schemaObject = a.Generate(typeof(TaskModel));




      var linkParser = new PropertyParsingRuleModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//a[contains(@class,'ellipsis-text')])[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.Attribute,
        OutputFromAttributeName = "href",
      };

      var nameParser = new PropertyParsingRuleModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-img-wrapper')])[1]/a[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.Attribute,
        OutputFromAttributeName = "name",
      };

      var priceParser = new PropertyParsingRuleModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-price')])[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.InnerText
      };


      var productParser = new PropertyParsingRuleModel {
        Type = OutputTypeEnum.Array,
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.CSS,
          MatchExpression = "div.product-list div.product-item"
        },
        PropertyParsingRules = new Dictionary<string, PropertyParsingRuleModel> {
          { "Link",linkParser },
          { "Name",nameParser },
          { "Price",priceParser }
        }
      };


      var taskModel = new TaskModel {
        Uri = "http://www.exdoll.com/productlist.ac",
        RequestMethod = RequestMethodEnum.Get,
        TaskId = 0,
        RequestParameter = new RequestParameterModel {
          Headers = new Dictionary<string, string> { { "cookie", "JSESSIONID=912BD825760319675E9DE1E1C1E2D701" } },
          Body = null
        },
        PropertyParsingRules = new Dictionary<string, PropertyParsingRuleModel> {
          { "Products",productParser }
        },
      };

      var stringEnumConverter = new JsonStringEnumConverter();
      JsonSerializerOptions opts = new JsonSerializerOptions();
      opts.IgnoreNullValues = true;
      opts.WriteIndented = true;
      //opts.Converters.Add(stringEnumConverter);


      var taskString = JsonSerializer.Serialize(taskModel);
      JObject taskModel1 = JObject.Parse(taskString);
      JSchema schema = JSchema.Parse(schemaObject.ToString());
      var valid = taskModel1.IsValid(schema);

    }
    [Fact]
    public async void KafkaSubscribeTest() {
      ConnectionFactory factory = new ConnectionFactory();
      // "guest"/"guest" by default, limited to localhost connections
      factory.UserName = "user";
      factory.Password = "test@test!";
      factory.VirtualHost = "/";
      factory.HostName = "mq.kirov-opensource.com";

      IConnection conn = factory.CreateConnection();
      IModel channel = conn.CreateModel();

      channel.QueueDeclare("hello", false, false, false, null);//创建一个名称为hello的消息队列
      for (int i = 0; i < 1000; i++) {
        string message = $"Hello World{i}"; //传递的消息内容
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish("", "hello", null, body); //开始传递  
        Console.WriteLine("已发送： {0}", message);
      }
      
      Console.ReadLine();

      channel.Close();
      conn.Close();
    }
    [Fact]
    public async void KafkaProduceTest() {
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
            Console.WriteLine("已接收： {0}", message);
            channel.BasicAck(ea.DeliveryTag, false);
          };
          Console.ReadLine();
        }
      }
    }
    [Fact]
    public async void ParserTest() {
      /*
       * 构思
       * 请求引擎：默认,WebDriver
       * 内容类型：Text,JSON,HTML
       * 内容字符集：UTF8,ASNII -restSharp有 不需要,
       * 
       * 使用QuartZ.NET启动一个Job 定时 批量 拉取MQ中的Task
       * 拉取后要将该Task状态置为执行中，如果达到超时阈值没有执行完毕，该状态重置
       * 在执行完毕后，将该Task的Response丢入MQ中并将该Task设置为执行完毕
       * 
       */


      var linkParser = new PropertyParsingRuleModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//a[contains(@class,'ellipsis-text')])[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.Attribute,
        OutputFromAttributeName = "href",
      };

      var nameParser = new PropertyParsingRuleModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-img-wrapper')])[1]/a[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.Attribute,
        OutputFromAttributeName = "name",
      };

      var priceParser = new PropertyParsingRuleModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-price')])[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.InnerText
      };


      var productParser = new PropertyParsingRuleModel {
        Type = OutputTypeEnum.Array,
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.CSS,
          MatchExpression = "div.product-list div.product-item"
        },
        PropertyParsingRules = new Dictionary<string, PropertyParsingRuleModel> {
          { "Link",linkParser },
          { "Name",nameParser },
          { "Price",priceParser }
        }
      };


      var taskModel = new TaskModel {
        Uri = "http://www.exdoll.com/productlist.ac",
        RequestMethod = RequestMethodEnum.Get,
        TaskId = 0,
        RequestParameter = new RequestParameterModel {
          Headers = new Dictionary<string, string> { { "cookie", "JSESSIONID=B8285D0CFC10BFE7B4EB27EB542C7743" } },
          Body = null
        },
        PropertyParsingRules = new Dictionary<string, PropertyParsingRuleModel> {
          { "Products",productParser }
        },
      };


      //ConnectionFactory factory = new ConnectionFactory();
      //// "guest"/"guest" by default, limited to localhost connections
      //factory.UserName = "user";
      //factory.Password = "test@test!";
      //factory.VirtualHost = "/";
      //factory.HostName = "mq.kirov-opensource.com";

      //IConnection conn = factory.CreateConnection();
      //IModel channel = conn.CreateModel();

      //channel.QueueDeclare("Spidernet_TaskQueue", false, false, false, null);//创建一个名称为hello的消息队列
      //for (int i = 0; i < 1000; i++) {
      //  string message = Newtonsoft.Json.JsonConvert.SerializeObject(taskModel); //传递的消息内容
      //  var body = Encoding.UTF8.GetBytes(message);
      //  channel.BasicPublish("", "Spidernet_TaskQueue", null, body); //开始传递  
      //  Console.WriteLine("已发送： {0}", message);
      //}

      var stringEnumConverter = new JsonStringEnumConverter();
      JsonSerializerOptions opts = new JsonSerializerOptions();
      opts.IgnoreNullValues = true;
      opts.WriteIndented = true;
      //opts.Converters.Add(stringEnumConverter);


      var txt = JsonSerializer.Serialize(taskModel, opts);
      //IRestClient restClient = new RestClient(taskModel.Uri);
      //IRestRequest request = new RestRequest(Method.GET);
      //request.AddCookie("JSESSIONID", "4173BE2521D676127C3F9C3F8EA68F67");
      //IRestResponse response = await restClient.ExecuteGetAsync(request);

      var response = await DownloadData(taskModel);
      var contentText = response.Content;
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(contentText);
      var rootNode = document.DocumentNode;
      var result = await Parse(rootNode, taskModel.PropertyParsingRules.First().Value);
      var resultT = new {
        TaskId = taskModel.TaskId,
        Result = result,
        ResponseHeaders = response.Headers.ToDictionary(c => c.Name, c => c?.Value?.ToString()),
        ResponseCookies = response.Cookies.ToDictionary(c => c.Name, c => c.Value)
      };
      var aaa = JsonSerializer.Serialize(resultT, opts);
    }

    private async Task<IRestResponse> DownloadData(TaskModel taskModel) {
      Method requestMethod = Method.GET;
      switch (taskModel.RequestMethod) {
        case RequestMethodEnum.Post:
          requestMethod = Method.POST;
          break;
        case RequestMethodEnum.Put:
          requestMethod = Method.PUT;
          break;
        case RequestMethodEnum.Patch:
          requestMethod = Method.PATCH;
          break;
        case RequestMethodEnum.Delete:
          requestMethod = Method.DELETE;
          break;
        case RequestMethodEnum.Copy:
          requestMethod = Method.COPY;
          break;
        case RequestMethodEnum.Merge:
          requestMethod = Method.MERGE;
          break;
        case RequestMethodEnum.Options:
          requestMethod = Method.OPTIONS;
          break;
        default:
        case RequestMethodEnum.None:
        case RequestMethodEnum.Get:
          requestMethod = Method.GET;
          break;
      }
      IRestClient restClient = new RestClient(taskModel.Uri);
      IRestRequest request = new RestRequest(requestMethod);
      //Fill Request Parameter And Header
      if (taskModel.RequestParameter.Headers?.Any() ?? false) {
        request.AddHeaders(taskModel.RequestParameter.Headers);
      }
      //if (taskModel.RequestParameter.Body) {
      //  request.AddBody(taskModel.RequestParameter.Body);
      //}
      return await restClient.ExecuteAsync(request, requestMethod);
    }

    private async Task<object> Parse(HtmlNode node, PropertyParsingRuleModel parser) {
      bool selectorIsXPath = parser.NodeSelector.Type == SelectorEnum.XPath;
      string selector = parser.NodeSelector.MatchExpression;
      object tempResult = null;
      switch (parser.Type) {
        case OutputTypeEnum.Text:
          var nodeInfo = selectorIsXPath ? node.SelectSingleNode(selector) : node.QuerySelector(selector);
          switch (parser.OutputFrom) {
            case OutputFromEnum.Attribute:
              tempResult = nodeInfo.GetAttributeValue(parser.OutputFromAttributeName, string.Empty);
              break;
            case OutputFromEnum.InnerHtml:
              tempResult = nodeInfo.InnerHtml;
              break;
            case OutputFromEnum.OuterHtml:
              tempResult = nodeInfo.OuterHtml;
              break;
            case OutputFromEnum.InnerLength:
              tempResult = nodeInfo.InnerLength;
              break;
            case OutputFromEnum.OuterLength:
              tempResult = nodeInfo.OuterLength;
              break;
            case OutputFromEnum.None:
            case OutputFromEnum.InnerText:
            default:
              tempResult = nodeInfo.InnerText;
              break;
          }
          break;
        case OutputTypeEnum.Array:
          var nodes = selectorIsXPath ? node.SelectNodes(selector) : node.QuerySelectorAll(selector);
          IList<object> tTempResult = new List<object>();
          //有Parser 即为对象
          if (parser.PropertyParsingRules?.Any() ?? false) {
            foreach (var tempNode in nodes) {
              dynamic tempDynamicResult = new ExpandoObject();
              var tempDynamicResultDic = (IDictionary<string, object>)tempDynamicResult;
              foreach (var tempParser in parser.PropertyParsingRules) {
                tempDynamicResultDic[tempParser.Key] = await Parse(tempNode, tempParser.Value);
              }
              tTempResult.Add(tempDynamicResult);
            }
            tempResult = tTempResult;
          }
          else {
            //无Parser 即为字符串数组
            tempResult = nodes.Select(c => c.InnerText);
          }
          break;
      }
      return tempResult;
    }
  }
}
