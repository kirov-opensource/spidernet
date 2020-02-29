using Snowflake.Core;
using Spidernet.DAL.Entities;

namespace Spidernet.DAL.Repositories
{
    public class TaskRepository : Repository<Task>
    {
        public TaskRepository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker) : base(dbConnectionFactory, idWorker)
        {
        }
    }
}
