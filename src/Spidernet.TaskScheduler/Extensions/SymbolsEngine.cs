using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Spidernet.TaskScheduler.Extensions {
  public static class SymbolsEngine {

    private const string symbolsRegex = "\\{\\{([a-z]|[A-Z]|[0-9]|:)*\\}\\}";

    /// <summary>
    /// Replace symbol with real value
    /// </summary>
    /// <param name="input">String with preprocessing symbols</param>
    /// <param name="parameters">Parameters in JSON format</param>
    /// <returns></returns>
    public static string SymbolsPreprocess(string input, string parameters) {
      var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(parameters))).Build();
      return SymbolsPreprocess(input, configuration);
    }

    /// <summary>
    /// Replace symbol with real value
    /// </summary>
    /// <param name="input">String with preprocessing symbols</param>
    /// <param name="configuration">Parameters in JSON format</param>
    /// <returns></returns>
    public static string SymbolsPreprocess(string input, IConfigurationRoot configuration) {
      return Regex.Replace(input, symbolsRegex, (match) => {
        //value = {{}}
        if (match.Value.Length == 4) {
          return string.Empty;
        }
        var key = match.Value.Substring(2, match.Value.Length - 4);
        var section = configuration.GetSection(key);
        if (section.Exists()) {
          return section.Value;
        }
        return string.Empty;
      });
    }
  }
}
