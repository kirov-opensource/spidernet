using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Options;
using RestSharp;
using Spidernet.BLL.Configs;
using Spidernet.Model.Enums;
using Spidernet.Model.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Spidernet.Client.Services {
  public class ClientService {
    public SpidernetClientConfig spidernetClientConfig;
    public ClientService(IOptionsMonitor<SpidernetClientConfig> spidernetClientConfig) {
      this.spidernetClientConfig = spidernetClientConfig.CurrentValue;
      spidernetClientConfig.OnChange<SpidernetClientConfig>(config => this.spidernetClientConfig = config);
    }

    public async Task<dynamic> ExecuteTask(TaskModel taskModel) {
      //获取数据和解析
      var response = await GetResponseAsync(taskModel);
      var contentText = response.Content;
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(contentText);
      var rootNode = document.DocumentNode;
      var resultObject = new ExpandoObject();
      var resultObjectDic = (IDictionary<string, object>)resultObject;
      foreach (var propertyParser in taskModel.PropertyParsingRules) {
        var propertyResponse = await HtmlParse(rootNode, propertyParser.Value);
        resultObjectDic[propertyParser.Key] = propertyResponse;
      }
      return resultObjectDic;
      //进行后续操作 好比丢入MQ
    }
    #region 扩展方法
    /// <summary>
    /// 解析HTML
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parser"></param>
    /// <returns></returns>
    private async Task<object> HtmlParse(HtmlNode node, PropertyParsingRuleModel parser) {
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
          if (parser.PropertyParsingRules?.Any() ?? false) {
            foreach (var tempNode in nodes) {
              dynamic tempDynamicResult = new ExpandoObject();
              var tempDynamicResultDic = (IDictionary<string, object>)tempDynamicResult;
              foreach (var tempParser in parser.PropertyParsingRules) {
                tempDynamicResultDic[tempParser.Key] = await HtmlParse(tempNode, tempParser.Value);
              }
              tTempResult.Add(tempDynamicResult);
            }
            tempResult = tTempResult;
          } else {
            tempResult = nodes.Select(c => c.InnerText);
          }
          break;
      }
      return tempResult;
    }
    /// <summary>
    /// 获取Response
    /// </summary>
    /// <param name="taskModel"></param>
    /// <returns></returns>
    private async Task<IRestResponse> GetResponseAsync(TaskModel taskModel) {
      var requestMethod = Method.GET;
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
      if (taskModel.RequestParameter.Headers?.Any() ?? false) {
        request.AddHeaders(taskModel.RequestParameter.Headers);
      }
      return await restClient.ExecuteAsync(request, requestMethod);
    }
    #endregion
  }
}
