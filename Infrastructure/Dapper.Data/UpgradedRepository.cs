using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dapper.Data
{
    public class UpgradedRepository
    {
        public UpgradedRepository()
        {

        }

        /// <summary>
        /// 根据条件筛选出数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(IDbConnection conn, string sql, dynamic param = null, bool buffered = true) where T : class
        {
            return SqlMapper.Query<T>(conn, sql, param as object, null, buffered);
        }
    }
}
