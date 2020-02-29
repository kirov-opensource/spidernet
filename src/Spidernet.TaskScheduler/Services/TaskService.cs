using Newtonsoft.Json;
using Spidernet.DAL.Entities;
using Spidernet.DAL.Repositories;
using Spidernet.Extension.Exceptions;
using Spidernet.Model.Tasks;
using System.Reflection;

namespace Spidernet.TaskScheduler.Services
{
    internal class TaskService : Service
    {

        private readonly TaskRepository taskRepository;
        private readonly TemplateRepository templateRepository;

        public TaskService(TaskRepository taskRepository, TemplateRepository templateRepository)
        {

            this.taskRepository = taskRepository;
            this.templateRepository = templateRepository;
        }


        internal TaskExecuteModel GenerateTask(Task task)
        {

            var template = templateRepository.FirstOrDefault(task.template_id);

            if (template == null)
            {
                throw new BusinessException("template can not be null.");
            }

            var parserObject = JsonConvert.DeserializeObject(template.parser);
            foreach (var fieldInfo in parserObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {

            }


            return null;
        }


    }
}
