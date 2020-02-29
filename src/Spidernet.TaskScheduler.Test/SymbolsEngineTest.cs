using Microsoft.Extensions.Configuration;
using Spidernet.TaskScheduler.Extensions;
using System.IO;
using System.Text;
using Xunit;

namespace Spidernet.TaskScheduler.Test {
  public class SymbolsEngineTest {


    [Fact]
    public void SymbolsPreprocessTest() {

      /* 
       The parameters json format:
       {
	        "Product": {
		        "Name": "product name",
		        "Id": 1
	        },
	        "Index": 109,
	        "Page": {
		        "PageIndex": 0,
		        "PageSize": 20
	        },
	        "ProductItems": ["ProductItem0", "ProductItem1", "ProductItem2"]
        }
       */

      var parameters = "{\"Product\":{\"Name\":\"product name\",\"Id\":1},\"Index\":109,\"Page\":{ \"PageIndex\":0,\"PageSize\":20},\"ProductItems\":[\"ProductItem0\",\"ProductItem1\",\"ProductItem2\"]}";
      var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(parameters))).Build();
      Assert.Equal("ProductName=product name", SymbolsEngine.SymbolsPreprocess("ProductName={{Product:Name}}", configuration));
      Assert.Equal("ProductItems=ProductItem1", SymbolsEngine.SymbolsPreprocess("ProductItems[1]={{ProductItems:1}}", configuration));
      //
      Assert.Equal("PageIndex=", SymbolsEngine.SymbolsPreprocess("PageIndex={{{Page:PageIndex}}}", configuration));
      Assert.Equal("Foo=", SymbolsEngine.SymbolsPreprocess("Foo={{Foo:Foo}}", configuration));
      Assert.Equal("PageNumber=", SymbolsEngine.SymbolsPreprocess("PageNumber={{Page:PageNumber}}", configuration));
    }

  }
}
