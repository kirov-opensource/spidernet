using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spidernet.Client.Enums;
using Spidernet.Client.Models;

namespace Spidernet.Client {
  public class Program {
    public static void Main(string[] args) {

      var linkParser = new ParserModel {
        Selector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(/a[contains(@classs,'ellipsis-text')])[1]/@href"
        },
        Type = Parser.String
      };

      var productNameParser = new ParserModel {
        Selector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(/div[contains(@classs,'pi-img-wrapper')])[1]/a[1]/@name"
        },
        Type = Parser.String
      };

      var priceParser = new ParserModel {
        Selector = new SelectorModel {
          Type = Selector.XPath,
          MatchExpression = "(/div[contains(@classs,'pi-price')])[1]/text()"
        },
        Type = Parser.String
      };


      var productParser = new ParserModel {
        Type = Parser.Array,
        Selector = new SelectorModel {
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

      var a = JsonSerializer.Serialize<TaskModel>(taskModel, opts);

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
              webBuilder.UseStartup<Startup>();
            });
  }
}
