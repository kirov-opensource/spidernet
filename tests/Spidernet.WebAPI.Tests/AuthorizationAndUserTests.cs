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
  public class AuthorizationAndUserTests : TestWithServer {
    private const string TOKEN_KEY = "user_token";
    private const string USERINFO_RESOURCE_PATH = "/api/session/user_info";

    [Theory]
    [InlineData("aaa", 1000000000000L)]
    public async Task 获取用户信息_传入TOKEN_返回TOKEN相关用户信息(string token, long expectUserId) {
      // 生成请求客户端
      var testClient = testServer.CreateClient();

      // 设置Token
      testClient.DefaultRequestHeaders.Add(TOKEN_KEY, token);

      // 获取响应
      var userInfoResponse = await testClient.GetAsync(USERINFO_RESOURCE_PATH);

      // 返回是正确
      Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);

      // 内容不为空
      var responseText = await userInfoResponse.Content.ReadAsStringAsync();
      Assert.False(string.IsNullOrWhiteSpace(responseText));

      // 转换成实体
      var userInfoEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(responseText);

      // 判断实体ID和期望的ID一致
      Assert.Equal(userInfoEntity.Id, expectUserId);
    }

    [Theory]
    [InlineData(1000000000000L)]
    public async Task 获取用户信息_不传入TOKEN_返回默认相关用户信息(long expectUserId) {
      // 生成请求客户端
      var testClient = testServer.CreateClient();

      // 获取响应
      var userInfoResponse = await testClient.GetAsync(USERINFO_RESOURCE_PATH);

      // 返回是正确
      Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);

      // 内容不为空
      var responseText = await userInfoResponse.Content.ReadAsStringAsync();
      Assert.False(string.IsNullOrWhiteSpace(responseText));

      // 转换成实体
      var userInfoEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(responseText);

      // 判断实体ID和期望的ID一致
      Assert.Equal(userInfoEntity.Id, expectUserId);
    }
  }
}
