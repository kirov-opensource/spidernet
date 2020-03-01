using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
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
      var linkParser = new PropertyParserModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//a[contains(@class,'ellipsis-text')])[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.Attribute,
        OutputFromAttributeName = "href",
      };

      var nameParser = new PropertyParserModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-img-wrapper')])[1]/a[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.Attribute,
        OutputFromAttributeName = "name",
      };

      var priceParser = new PropertyParserModel {
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-price')])[1]"
        },
        Type = OutputTypeEnum.Text,
        OutputFrom = OutputFromEnum.InnerText
      };


      var productParser = new PropertyParserModel {
        Type = OutputTypeEnum.Array,
        NodeSelector = new SelectorModel {
          Type = SelectorEnum.CSS,
          MatchExpression = "div.product-list div.product-item"
        },
        PropertyParsers = new Dictionary<string, PropertyParserModel> {
          { "Link",linkParser },
          { "Name",nameParser },
          { "Price",priceParser }
        }
      };


      var taskModel = new TaskModel {
        Uri = "http://www.exdoll.com/productlist.ac",
        TaskId = 0,
        Variables = new Dictionary<string, string> { },
        PropertyParsers = new Dictionary<string, PropertyParserModel> {
          { "Products",productParser }
        },
      };

      var stringEnumConverter = new JsonStringEnumConverter();
      JsonSerializerOptions opts = new JsonSerializerOptions();
      opts.IgnoreNullValues = true;
      opts.WriteIndented = true;
      opts.Converters.Add(stringEnumConverter);


      var txt = JsonSerializer.Serialize(taskModel, opts);
      IRestClient restClient = new RestClient(taskModel.Uri);
      IRestRequest request = new RestRequest(Method.GET);
      request.AddCookie("JSESSIONID", "4173BE2521D676127C3F9C3F8EA68F67");
      IRestResponse response = await restClient.ExecuteGetAsync(request);
      var contentText = response.Content;
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(contentText);
      var rootNode = document.DocumentNode;
      var result = await Parse(rootNode, taskModel.PropertyParsers.First().Value);
    }

    private async Task<object> Parse(HtmlNode node, PropertyParserModel parser) {
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
