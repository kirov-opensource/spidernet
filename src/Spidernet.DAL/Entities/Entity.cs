using Dapper.Contrib.Extensions;
using System;

namespace Spidernet.DAL.Entities
{
    public abstract class Entity
    {
        /// <summary>
        /// 标识列
        /// </summary>
        [ExplicitKey]
        public long id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_at { get; set; } = DateTime.Now;

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? delete_at { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime update_at { get; set; } = DateTime.Now;

    }
}
