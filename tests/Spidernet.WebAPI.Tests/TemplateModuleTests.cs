using Spidernet.BLL.Models.Templates;
using Spidernet.Model.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Spidernet.WebAPI.Tests {
  public class TemplateModuleTests : TestWithServer {
    private const string UPDATE_TEMPLATE_RESOURCE_PATH = "/api/template/{0}";
    private const string CREATE_TEMPLATE_RESOURCE_PATH = "/api/template";
    private const string GET_TEMPLATE_RESOURCE_PATH = "/api/template/{0}";

    [Fact]
    public async Task 创建模板_返回无内容状态码() {
      // 创建客户端
      var client = testServer.CreateClient();

      string tick = DateTime.Now.Ticks.ToString();

      // 创建模型
      var createTemplateModel = new CreateTemplateModel();
      createTemplateModel.Name = $"单元测试{tick}";
      createTemplateModel.No = $"UnitTest_{tick}";
      createTemplateModel.Uri = "https://www.baidu.com";
      createTemplateModel.PropertyParsingRule = new Model.Models.PropertyParsingRuleModel {

      };

      // 生成请求内容
      var postContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(createTemplateModel), Encoding.UTF8, "application/json");

      // 发送请求
      var templateModel = await client.PostAsync(CREATE_TEMPLATE_RESOURCE_PATH, postContent);

      // 成功的状态码就是204
      Assert.Equal(HttpStatusCode.NoContent, templateModel.StatusCode);
    }
    /// <summary>
    /// 获取模板_传入编码_返回相应模板
    /// </summary>
    /// <param name="no">编码</param>
    /// <param name="expectedId">期望ID</param>
    /// <returns></returns>
    [Theory]
    [InlineData("SBWSJ", 1257289584698593280L)]
    [InlineData("UnitTest_637242239784070056", 1257298872913498112L)]
    public async Task 获取模板_传入编码_返回模板和期望ID一致(string no, long expectedId) {
      // 创建客户端
      var client = testServer.CreateClient();

      // 将Path生成
      var combineGetTemplatePath = string.Format(GET_TEMPLATE_RESOURCE_PATH, no);

      // 发送请求
      var templateResponse = await client.GetAsync(combineGetTemplatePath);

      // 响应状态码正确
      Assert.Equal(HttpStatusCode.OK, templateResponse.StatusCode);

      // 转换为模板模型
      var templateModel = await templateResponse.Content.Cast<TemplateDetailViewModel>();

      // 判断模型ID和期望ID一致
      Assert.Equal(expectedId, templateModel?.Id);
    }

    [Theory]
    [InlineData("SBWSJ", 1257289584698593280L)]
    public async Task 修改模板_传入模板编码_修改返回的模板(string no, long expectedId) {
      // 创建客户端
      var client = testServer.CreateClient();

      // 将Path生成
      var combineGetTemplatePath = string.Format(GET_TEMPLATE_RESOURCE_PATH, no);

      // 发送请求
      var templateResponse = await client.GetAsync(combineGetTemplatePath);

      // 响应状态码正确
      Assert.Equal(HttpStatusCode.OK, templateResponse.StatusCode);

      // 转换为模板模型
      var templateModel = await templateResponse.Content.Cast<TemplateDetailViewModel>();

      // 判断模型ID和期望ID一致
      Assert.Equal(expectedId, templateModel?.Id);

      // 设置更新内容
      templateModel.Uri = "http://www.exdoll.com/productlist.html";
      templateModel.PropertyParsingRule = new PropertyParsingRuleModel {
        Type = Model.Enums.OutputTypeEnum.Array,
        NodeSelector = new SelectorModel {
          Type = Model.Enums.SelectorEnum.CSS,
          MatchExpression = ".row.product-list .col-xs-3"
        },
        PropertyParsingRules = new Dictionary<string, PropertyParsingRuleModel>() {
          { "product_name",new PropertyParsingRuleModel{
            NodeSelector = new SelectorModel{
              Type = Model.Enums.SelectorEnum.CSS,
              MatchExpression = ".pi-img-wrapper a"
            },
            OutputFrom = Model.Enums.OutputFromEnum.Attribute,
            OutputFromAttributeName = "name"
          } },
          { "product_price",new PropertyParsingRuleModel{
            NodeSelector = new SelectorModel{
              Type = Model.Enums.SelectorEnum.CSS,
              MatchExpression = ".pi-price"
            },
            OutputFrom = Model.Enums.OutputFromEnum.InnerText,
          } },
          { "product_detail_uri", new PropertyParsingRuleModel{
            NodeSelector = new SelectorModel{
              Type = Model.Enums.SelectorEnum.CSS,
              MatchExpression = ".pi-img-wrapper a"
            },
            OutputFrom = Model.Enums.OutputFromEnum.Attribute,
            OutputFromAttributeName = "href"
          } },
          { "product_image_uri", new PropertyParsingRuleModel{
            NodeSelector = new SelectorModel{
              Type = Model.Enums.SelectorEnum.CSS,
              MatchExpression = ".pi-img-wrapper a img"
            },
            OutputFrom = Model.Enums.OutputFromEnum.Attribute,
            OutputFromAttributeName = "src"
          } }
        }
      };

      var combineUpdateTemplatePath = string.Format(UPDATE_TEMPLATE_RESOURCE_PATH, no);

      // 生成请求内容
      var updateContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(templateModel), Encoding.UTF8, "application/json");

      // 发送请求
      var createdResponse = await client.PutAsync(combineUpdateTemplatePath, updateContent);

      // 成功的状态码就是204
      Assert.Equal(HttpStatusCode.NoContent, createdResponse.StatusCode);
    }
  }
}
