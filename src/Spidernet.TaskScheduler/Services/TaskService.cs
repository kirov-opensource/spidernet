using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spidernet.DAL.Entities;
using Spidernet.DAL.Repositories;
using Spidernet.Extension;
using Spidernet.Model.Enums;
using Spidernet.Model.Models;
using Spidernet.TaskScheduler.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spidernet.TaskScheduler.Services {
  public class TaskService : Service {

    private readonly TaskRepository taskRepository;
    private readonly TemplateRepository templateRepository;

    public TaskService(TaskRepository taskRepository, TemplateRepository templateRepository) {

      this.taskRepository = taskRepository;
      this.templateRepository = templateRepository;
    }

    /// <summary>
    /// Generate task
    /// </summary>
    /// <param name="task"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public TaskModel GenerateTask(Task task, Template template) {
      if (template == null || task == null) {
        throw new ArgumentNullException(template == null ? nameof(template) : nameof(task));
      }
      var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(task.parameters))).Build();
      Enum.TryParse(task.http_method.ToPascalCase(), out RequestMethodEnum requestMethod);
      var taskExecuteModel = new TaskModel {
        RequestMethod = requestMethod,
        PropertyParsingRules = JsonConvert.DeserializeObject<IDictionary<string, PropertyParsingRuleModel>>(SymbolsEngine.SymbolsPreprocess(JsonConvert.SerializeObject(template.property_parsing_rule), configuration)),
        TaskId = task.id,
        Uri = SymbolsEngine.SymbolsPreprocess(template.uri, configuration)
      };
      return taskExecuteModel;
    }
  }
}
