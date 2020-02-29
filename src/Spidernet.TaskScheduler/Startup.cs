using Spidernet.DAL.Repositories;

namespace Spidernet.TaskScheduler
{
    internal class Startup
    {
        private readonly TaskRepository taskRepository;
        private readonly TemplateRepository templateRepository;

        public Startup(TaskRepository taskRepository, TemplateRepository templateRepository)
        {

            this.taskRepository = taskRepository;
            this.templateRepository = templateRepository;
        }




    }
}
