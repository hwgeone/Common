using Infrastructure.Dapper.Extensions;
using Infrastructure.Dapper.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dapper.Data
{
    /// <summary>
    /// 只读不写
    /// </summary>
    public class ConnectionProvidor
    {
        /// <summary>
        /// 创建类的构造方法
        /// </summary>
        public ConnectionProvidor(DBTypeEnum type)
        {
            InitializeDBType(type);     //多数据库枚举类型，ORACLE, MYSQL等
        }

        /// <summary>
        /// 设置数据库类型相关的变量
        /// </summary>
        /// <param name="type">数据库类型</param>
        private void InitializeDBType(DBTypeEnum type)
        {
            DBTypeExtenstions.SetDBType(type);      
        }

        private IDbConnection CreateConnectionByDBType(string connectionStringName)
        {
            IDbConnection conn = null;
            var connStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (DBTypeExtenstions.DBType == DBTypeEnum.SQLSERVER)
            {
                conn = new SqlConnection(connStringSetting.ConnectionString);
            }
            else if (DBTypeExtenstions.DBType == DBTypeEnum.ORACLE)
            {
                conn = new OracleConnection(connStringSetting.ConnectionString);
            }
            else if (DBTypeExtenstions.DBType == DBTypeEnum.MYSQL)
            {
                //conn = new MySqlConnection(connStringSetting.ConnectionString);
            }
            return conn;
        }


        public IDbConnection CreateConnection(string connectionStringName)
        {
            IDbConnection conn = CreateConnectionByDBType(connectionStringName);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }
    }
}
