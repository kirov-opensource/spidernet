using Spidernet.BLL.Models.Templates;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Spidernet.WebAPI.Tests {
  public class TemplateModuleTests : TestWithServer {
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
      var createdResponse = await client.PostAsync(CREATE_TEMPLATE_RESOURCE_PATH, postContent);

      // 成功的状态码就是204
      Assert.Equal(HttpStatusCode.NoContent, createdResponse.StatusCode);
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
    public async Task 获取模板_传入编码_返回相应模板(string no, long expectedId) {
      // 创建客户端
      var client = testServer.CreateClient();
      // 将Path生成
      var combinePath = string.Format(GET_TEMPLATE_RESOURCE_PATH, no);


      // 发送请求
      var getResponse = await client.GetAsync(combinePath);

      Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

      var responseTest = await getResponse.Content.ReadAsStringAsync();

      var responseTemplate = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateDetailViewModel>(responseTest);

      Assert.Equal(expectedId, responseTemplate.Id);
    }
  }
}
