using Microsoft.Extensions.Configuration;
using Spidernet.DAL.Entities;
using Spidernet.DAL.Repositories;
using Spidernet.Extension.Exceptions;
using Spidernet.Model.Tasks;
using Spidernet.TaskScheduler.Extensions;
using System.IO;
using System.Text;

namespace Spidernet.TaskScheduler.Services {
  internal class TaskService : Service {

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
    /// <returns></returns>
    internal TaskExecuteModel GenerateTask(Task task) {
      var template = templateRepository.FirstOrDefault(task.template_id);
      if (template == null) {
        throw new BusinessException("template can not be null.");
      }
      var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(task.parameters))).Build();
      var taskExecuteModel = new TaskExecuteModel {
        Header = SymbolsEngine.SymbolsPreprocess(task.header, configuration),
        Parser = SymbolsEngine.SymbolsPreprocess(template.parser, configuration),
        TaskId = task.id,
        Uri = SymbolsEngine.SymbolsPreprocess(task.uri, configuration)
      };
      return taskExecuteModel;
    }



  }
}
