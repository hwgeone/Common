using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dapper.Data
{
    public class DBFactory
    {
        private DBTypeEnum dbType;

        public DBFactory()
        {
            dbType = DBTypeEnum.SQLSERVER;
        }

        public DBFactory(DBTypeEnum type)
        {
            dbType = type;
        }

        public IDbConnection GetDbConnection(string dbString)
        {
            return new ConnectionProvidor(dbType).CreateConnection(dbString);
        }
    }
}
