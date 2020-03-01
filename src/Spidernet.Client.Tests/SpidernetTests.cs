using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using RestSharp;
using Spidernet.Model.Enums;
using Spidernet.Model.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Spidernet.Client.Tests {
  public class SpidernetTests {

    [Fact]
    public async void ParserTest() {
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
        PropertyParsers = new Dictionary<string, PropertyParsingRuleModel> {
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
          Headers = new Dictionary<string, string> { { "JSESSIONID", "4173BE2521D676127C3F9C3F8EA68F67" } },
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
      opts.Converters.Add(stringEnumConverter);


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
          if (parser.PropertyParsers?.Any() ?? false) {
            foreach (var tempNode in nodes) {
              dynamic tempDynamicResult = new ExpandoObject();
              var tempDynamicResultDic = (IDictionary<string, object>)tempDynamicResult;
              foreach (var tempParser in parser.PropertyParsers) {
                tempDynamicResultDic[tempParser.Key] = await Parse(tempNode, tempParser.Value);
              }
              tTempResult.Add(tempDynamicResult);
            }
            tempResult = tTempResult;
          } else {
            //无Parser 即为字符串数组
            tempResult = nodes.Select(c => c.InnerText);
          }
          break;
      }
      return tempResult;
    }
  }
}
