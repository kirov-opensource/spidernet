using Spidernet.DAL.Entities;
using Spidernet.Model.Enums;
using Spidernet.Model.Models;
using Spidernet.TaskScheduler.Services;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Spidernet.TaskScheduler.Tests
{
    public class TaskTest
    {


        [Fact]
        public void GenerateTaskTest()
        {

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
            var taskService = new TaskService(null, null);
            var task = new Task
            {
                header = string.Empty,
                http_method = HttpMethod.Get.Method.ToLower(),
                uri = string.Empty,
                parameters = "{\"Google\":{\"SearchString\":\"how+use+google+search+engine?\"}}"
            };

            var searchString = new PropertyParsingRuleModel
            {
                NodeSelector = new SelectorModel
                {
                    Type = SelectorEnum.XPath,
                    MatchExpression = "XPathValue:{{Google:SearchString}}"
                },
                Type = OutputTypeEnum.Text
            };

            var id = new PropertyParsingRuleModel
            {
                NodeSelector = new SelectorModel
                {
                    Type = SelectorEnum.XPath,
                    MatchExpression = string.Empty
                },
                Type = OutputTypeEnum.Text
            };

            var googleEngine = new PropertyParsingRuleModel
            {
                Type = OutputTypeEnum.Text,
                NodeSelector = new SelectorModel
                {
                    Type = SelectorEnum.XPath,
                    MatchExpression = string.Empty
                },
                PropertyParsingRules = new Dictionary<string, PropertyParsingRuleModel> { { "SearchString", searchString }, { "Id", id } }
            };


            var template = new Template
            {
                header = new Newtonsoft.Json.Linq.JObject { },
                subsequent_task_property_scheme = new Newtonsoft.Json.Linq.JObject { },
                property_parsing_rule = new Dictionary<string, PropertyParsingRuleModel> { { "GoogleEngine", googleEngine } },
                uri = "https://www.google.com/search?q={{Google:SearchString}}"
            };

            var taskModel = taskService.GenerateTask(task, template);
            Assert.Equal("https://www.google.com/search?q=how+use+google+search+engine?", taskModel.Uri);
            Assert.Null(taskModel.RequestParameter);
            Assert.Equal(RequestMethodEnum.Get, taskModel.RequestMethod);
            Assert.Equal("XPathValue:how+use+google+search+engine?", taskModel.PropertyParsingRules["GoogleEngine"].PropertyParsingRules["SearchString"].NodeSelector.MatchExpression);
        }

    }
}
