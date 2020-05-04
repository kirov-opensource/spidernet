using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using Spidernet.BLL.Models.Users;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Spidernet.WebAPI.Tests {
  public class AuthorizationAndUserTests {
    private const string TOKEN_KEY = "user_token";
    private const string USERINFO_RESOURCE_PATH = "/api/session/user_info";
    private TestServer testServer;
    public AuthorizationAndUserTests() {

      var webHostBuilder = WebHost.CreateDefaultBuilder(new string[] { }).UseStartup<Startup>();

      TestServer testServer = new TestServer(webHostBuilder);
      this.testServer = testServer;

    }

    [Theory]
    [InlineData("aaa", 1000000000000L)]
    public async Task ��ȡ�û���Ϣ_����TOKEN_����TOKEN����û���Ϣ(string token, long exceptUserId) {
      // ��������ͻ���
      var testClient = testServer.CreateClient();
      // ����Token
      testClient.DefaultRequestHeaders.Add(TOKEN_KEY, token);
      // ��ȡ��Ӧ
      var userInfoResponse = await testClient.GetAsync(USERINFO_RESOURCE_PATH);
      // ��������ȷ
      Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);
      // ���ݲ�Ϊ��
      var responseText = await userInfoResponse.Content.ReadAsStringAsync();

      Assert.False(string.IsNullOrWhiteSpace(responseText));
      // ת����ʵ��
      var userInfoEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(responseText);

      // �ж�ʵ��ID��������IDһ��
      Assert.Equal(userInfoEntity.Id, exceptUserId);
    }

    [Theory]
    [InlineData(1000000000000L)]
    public async Task ��ȡ�û���Ϣ_������TOKEN_����Ĭ������û���Ϣ(long exceptUserId) {
      // ��������ͻ���
      var testClient = testServer.CreateClient();
      // ��ȡ��Ӧ
      var userInfoResponse = await testClient.GetAsync(USERINFO_RESOURCE_PATH);
      // ��������ȷ
      Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);
      // ���ݲ�Ϊ��
      var responseText = await userInfoResponse.Content.ReadAsStringAsync();

      Assert.False(string.IsNullOrWhiteSpace(responseText));
      // ת����ʵ��
      var userInfoEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(responseText);

      // �ж�ʵ��ID��������IDһ��
      Assert.Equal(userInfoEntity.Id, exceptUserId);
    }
  }
}
