using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Spidernet.TaskScheduler.Extensions {
  public static class SymbolsEngine {

    /// <summary>
    /// Replace symbol with real value
    /// </summary>
    /// <param name="input">String with preprocessing symbols</param>
    /// <param name="parameters">Parameters in JSON format</param>
    /// <returns></returns>
    public static string SymbolsPreprocess(string input, string parameters) {
      var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(parameters))).Build();
      return Regex.Replace(input, "", (match) => {
        var section = configuration.GetSection(match.Value);
        if (section.Exists()) {
          return section.Value;
        }
        return match.Value;
      });
    }

    /// <summary>
    /// Replace symbol with real value
    /// </summary>
    /// <param name="input">String with preprocessing symbols</param>
    /// <param name="configuration">Parameters in JSON format</param>
    /// <returns></returns>
    public static string SymbolsPreprocess(string input, IConfigurationRoot configuration) {
      return Regex.Replace(input, "", (match) => {
        var section = configuration.GetSection(match.Value);
        if (section.Exists()) {
          return section.Value;
        }
        return match.Value;
      });
    }
  }
}
