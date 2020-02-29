using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using RestSharp;
using Spidernet.Client.Enums;
using Spidernet.Client.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Spidernet.Client.Tests {
  public class SpidernetTests {
    [Fact]
    public void SerializeAndDeserializeTest() {
      var linkParser = new ParserModel {
        NodeSelector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(/a[contains(@classs,'ellipsis-text')])[1]/@href"
        },
        Type = Parser.String
      };

      var productNameParser = new ParserModel {
        NodeSelector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(/div[contains(@classs,'pi-img-wrapper')])[1]/a[1]/@name"
        },
        Type = Parser.String
      };

      var priceParser = new ParserModel {
        NodeSelector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(/div[contains(@classs,'pi-price')])[1]/text()"
        },
        Type = Parser.String
      };


      var productParser = new ParserModel {
        Type = Parser.Array,
        NodeSelector = new SelectorModel {
          Type = Selector.CSS,
          MatchExpression = ".product-list>.product-item"
        },
        Parser = new Dictionary<string, ParserModel> {
          { "Link",linkParser },
          { "Name",productNameParser },
          { "Price",priceParser }
        }
      };


      var taskModel = new TaskModel {
        Uri = "http://www.exdoll.com/productlist.ac",
        TaskId = 0,
        Variables = new Dictionary<string, string> { },
        Parser = new Dictionary<string, ParserModel> {
          { "Products",productParser }
        },
      };
      var stringEnumConverter = new System.Text.Json.Serialization.JsonStringEnumConverter();
      JsonSerializerOptions opts = new JsonSerializerOptions();
      opts.IgnoreNullValues = true;
      opts.Converters.Add(stringEnumConverter);

      var serializeString = JsonSerializer.Serialize<TaskModel>(taskModel, opts);
      var deserializeString = "{\"TaskId\":0,\"Uri\":\"http://www.exdoll.com/productlist.ac\",\"Parser\":{\"Products\":{\"Type\":\"Array\",\"Selector\":{\"Type\":\"CSS\",\"MatchExpression\":\".product-list\u003E.product-item\"},\"Parser\":{\"Link\":{\"Type\":\"String\",\"Selector\":{\"Type\":\"XPath\",\"MatchExpression\":\"(/a[contains(@classs,\u0027ellipsis-text\u0027)])[1]/@href\"}},\"Name\":{\"Type\":\"String\",\"Selector\":{\"Type\":\"XPath\",\"MatchExpression\":\"(/div[contains(@classs,\u0027pi-img-wrapper\u0027)])[1]/a[1]/@name\"}},\"Price\":{\"Type\":\"String\",\"Selector\":{\"Type\":\"XPath\",\"MatchExpression\":\"(/div[contains(@classs,\u0027pi-price\u0027)])[1]/text()\"}}}}},\"Variables\":{}}";
      Assert.Equal(System.Text.RegularExpressions.Regex.Unescape(serializeString), deserializeString);
      var deserializeObject = JsonSerializer.Deserialize<TaskModel>(deserializeString, opts);
      var serializeStringDeserializeObject = JsonSerializer.Deserialize<TaskModel>(serializeString, opts);
    }

    [Fact]
    public async void ParserTest() {
      //var stringEnumConverter = new System.Text.Json.Serialization.JsonStringEnumConverter();
      //JsonSerializerOptions opts = new JsonSerializerOptions();
      //opts.IgnoreNullValues = true;
      //opts.Converters.Add(stringEnumConverter);
      //var parserString = "{\"TaskId\":0,\"Uri\":\"http://www.exdoll.com/productlist.ac\",\"Parser\":{\"Products\":{\"Type\":\"Array\",\"Selector\":{\"Type\":\"CSS\",\"MatchExpression\":\".product-list\u003E.product-item\"},\"Parser\":{\"Link\":{\"Type\":\"String\",\"Selector\":{\"Type\":\"XPath\",\"MatchExpression\":\"(/a[contains(@classs,\u0027ellipsis-text\u0027)])[1]/@href\"}},\"Name\":{\"Type\":\"String\",\"Selector\":{\"Type\":\"XPath\",\"MatchExpression\":\"(/div[contains(@classs,\u0027pi-img-wrapper\u0027)])[1]/a[1]/@name\"}},\"Price\":{\"Type\":\"String\",\"Selector\":{\"Type\":\"XPath\",\"MatchExpression\":\"(/div[contains(@classs,\u0027pi-price\u0027)])[1]/text()\"}}}}},\"Variables\":{}}";
      ////div.product-list div.product-item
      //var parserModel = JsonSerializer.Deserialize<TaskModel>(parserString, opts);
      var linkParser = new ParserModel {
        NodeSelector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(.//a[contains(@class,'ellipsis-text')])[1]"
        },
        Type = Parser.String,
        OutputFrom = OutputFrom.Attribute,
        OutputFromAttributeName = "href",
      };

      var nameParser = new ParserModel {
        NodeSelector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-img-wrapper')])[1]/a[1]"
        },
        Type = Parser.String,
        OutputFrom = OutputFrom.Attribute,
        OutputFromAttributeName = "name",
      };

      var priceParser = new ParserModel {
        NodeSelector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(.//div[contains(@class,'pi-price')])[1]"
        },
        Type = Parser.String,
        OutputFrom = OutputFrom.InnerText
      };


      var productParser = new ParserModel {
        Type = Parser.Array,
        NodeSelector = new SelectorModel {
          Type = Selector.CSS,
          MatchExpression = "div.product-list div.product-item"
        },
        Parser = new Dictionary<string, ParserModel> {
          { "Link",linkParser },
          { "Name",nameParser },
          { "Price",priceParser }
        }
      };


      var taskModel = new TaskModel {
        Uri = "http://www.exdoll.com/productlist.ac",
        TaskId = 0,
        Variables = new Dictionary<string, string> { },
        Parser = new Dictionary<string, ParserModel> {
          { "Products",productParser }
        },
      };



      IRestClient restClient = new RestClient(taskModel.Uri);
      IRestRequest request = new RestRequest(Method.GET);
      request.AddCookie("JSESSIONID", "C2F015054B04DD036D311D93A8CAF48E");
      IRestResponse response = await restClient.ExecuteGetAsync(request);
      var contentText = response.Content;
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(contentText);
      var rootNode = document.DocumentNode;
      var result = await Parse(rootNode, taskModel.Parser.First().Value);
    }

    private async Task<object> Parse(HtmlNode node, ParserModel parser) {
      bool selectorIsXPath = parser.NodeSelector.Type == Selector.XPath;
      string selector = parser.NodeSelector.MatchExpression;
      object tempResult = null;
      switch (parser.Type) {
        case Parser.Raw:
          tempResult = selectorIsXPath ? node.SelectSingleNode(selector) : node.QuerySelector(selector);
          break;
        case Parser.String:
          var nodeInfo = selectorIsXPath ? node.SelectSingleNode(selector) : node.QuerySelector(selector);
          switch (parser.OutputFrom) {
            case OutputFrom.Attribute:
              tempResult = nodeInfo.GetAttributeValue(parser.OutputFromAttributeName, string.Empty);
              break;
            case OutputFrom.InnerHtml:
              tempResult = nodeInfo.InnerHtml;
              break;
            case OutputFrom.OuterHtml:
              tempResult = nodeInfo.OuterHtml;
              break;
            case OutputFrom.InnerLength:
              tempResult = nodeInfo.InnerLength;
              break;
            case OutputFrom.OuterLength:
              tempResult = nodeInfo.OuterLength;
              break;
            case OutputFrom.None:
            case OutputFrom.InnerText:
            default:
              tempResult = nodeInfo.InnerText;
              break;
          }
          break;
        case Parser.Array:
          var nodes = selectorIsXPath ? node.SelectNodes(selector) : node.QuerySelectorAll(selector);
          IList<object> tTempResult = new List<object>();
          //有Parser 即为对象
          if (parser.Parser?.Any() ?? false) {
            foreach (var tempNode in nodes) {
              dynamic tempDynamicResult = new ExpandoObject();
              var tempDynamicResultDic = (IDictionary<string, object>)tempDynamicResult;
              foreach (var tempParser in parser.Parser) {
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
        case Parser.None:
          throw new Exception("ABCDE");
          break;
      }
      return tempResult;
    }
  }
}
