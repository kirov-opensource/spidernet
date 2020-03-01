using Spidernet.DAL.Entities;
using Spidernet.Model.Enums;
using Spidernet.TaskScheduler.Services;
using System.Net.Http;
using Xunit;

namespace Spidernet.TaskScheduler.Tests {
  public class TaskTest {


    [Fact]
    public void GenerateTaskTest() {

      var taskService = new TaskService(null, null);

      /* 
        The parameters json format:
        {
	        "Google": {
		        "SearchString": "how+use+google+search+engine?"
	        }
        }
       */

      /*
       * 
        The property parsing rule format:
        {
	        "GoogleEngine": {
		        "Type": "Text",
		        "NodeSelector": {
			        "Type": "XPath",
			        "MatchExpression": "expression"
		        },
		        "PropertyParsingRules": {
			        "SearchString": {
				        "Type": "Text",
				        "NodeSelector": {
					        "Type": "XPath",
					        "MatchExpression": "{{Google:SearchString}}"
				        }
			        },
			        "Id": {
				        "Type": "Text",
				        "NodeSelector": {
					        "Type": "XPath",
					        "MatchExpression": "xxxx"
				        }
			        }
		        }
	        }
        }
       */

      var task = new Task {
        header = string.Empty,
        http_method = HttpMethod.Get.Method.ToLower(),
        uri = string.Empty,
        parameters = "{\"Google\":{\"SearchString\":\"how+use+google+search+engine?\"}}"
      };

      var template = new Template {
        header = string.Empty,
        subsequent_task_property_scheme = string.Empty,
        property_parsing_rule = "{\"GoogleEngine\":{ \"Type\":\"Text\",\"NodeSelector\":{\"Type\":\"XPath\",\"MatchExpression\":\"expression\"},\"PropertyParsingRules\":{\"SearchString\":{\"Type\":\"Text\",\"NodeSelector\":{\"Type\":\"XPath\",\"MatchExpression\":\"{{Google:SearchString}}\"}},\"Id\":{ \"Type\":\"Text\",\"NodeSelector\":{\"Type\":\"XPath\",\"MatchExpression\":\"xxxx\"}}}}}",
        uri = "https://www.google.com/search?q={{Google:SearchString}}"
      };

      var taskModel = taskService.GenerateTask(task, template);
      Assert.Equal("https://www.google.com/search?q=how+use+google+search+engine?", taskModel.Uri);
      Assert.Null(taskModel.RequestParameter);
      Assert.Equal(RequestMethodEnum.Get, taskModel.RequestMethod);
      Assert.Equal("how+use+google+search+engine?", taskModel.PropertyParsingRules["GoogleEngine"].PropertyParsingRules["SearchString"].NodeSelector.MatchExpression);
    }

  }
}
