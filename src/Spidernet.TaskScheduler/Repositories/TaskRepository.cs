using Snowflake.Core;
using Spidernet.TaskScheduler.Entities;

namespace Spidernet.TaskScheduler.Repositories
{
    internal class TaskRepository : Repository<Task>
    {
        internal TaskRepository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker) : base(dbConnectionFactory, idWorker)
        {
        }
    }
}
