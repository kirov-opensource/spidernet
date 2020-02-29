using Snowflake.Core;
using Spidernet.DAL.Entities;

namespace Spidernet.DAL.Repositories
{
    public class TemplateRepository : Repository<Template>
    {
        public TemplateRepository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker) : base(dbConnectionFactory, idWorker)
        {

        }
    }
}
