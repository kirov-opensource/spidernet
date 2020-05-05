using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Spidernet.WebAPI.Tests {
  public static class HttpContentExtensions {
    public static async Task<T> Cast<T>(this HttpContent httpContent) where T : class, new() {
      var responseText = await httpContent.ReadAsStringAsync();
      if (!string.IsNullOrEmpty(responseText)) {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
      }
      return null;
    }
  }
}
