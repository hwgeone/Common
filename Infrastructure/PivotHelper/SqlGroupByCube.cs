using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.Common;

using NReco.PivotData;
using NReco.PivotData.Input;

namespace Services
{

    /// <summary>
    /// Implements data cube for SQL data that performs aggregation with GROUP BY query on-the-fly.
    /// </summary>
    public class SqlGroupByCube : ICube
    {

        public string Id { get; private set; }

        public string Name { get; set; }

        IDbConnection Connection;
        string SelectSql;
        PivotDataConfiguration PvtCfg;

        public PivotDataFactory PvtDataFactory { get; set; }

        Dictionary<string, string> AggregatorSqlTemplates;

        /// <summary>
        /// Initializes <see cref="SqlGroupByCube"/> instance.
        /// </summary>
        /// <param name="id">cube name (unique identifier)</param>
        /// <param name="pvtCfg">cube configuration: available dimensions and measures</param>
        /// <param name="connection">ADO.NET database connection instance</param>
        /// <param name="selectSql">select that returns all columns that may be used as dimensions or as measures</param>
        public SqlGroupByCube(string id, PivotDataConfiguration pvtCfg, IDbConnection connection, string selectSql)
        {
            Id = id;
            Name = id;
            PvtCfg = pvtCfg;
            Connection = connection;
            SelectSql = selectSql;

            PvtDataFactory = new PivotDataFactory();

            // aggregator names supported by this implementation
            // the following aggregator names are supported by GroupedSourceReader by default
            // If you need to add additional aggregator types: https://www.nrecosite.com/pivotdata/load-pre-aggregated-data.aspx
            AggregatorSqlTemplates = new Dictionary<string, string>() {
				{"Count", null},
				{"Sum", "SUM({0})" },
				{"Average", "AVG({0})" },
				{"Min", "MIN({0})"},
				{"Max", "MAX({0})"}
			};
        }

        public PivotDataConfiguration GetConfiguration()
        {
            return PvtCfg;
        }

        /// <summary>
        /// This method composes SQL GROUP BY command only for specified dimensions/aggregators according to naming convention 
        /// used by default GroupedSourceReader configuration.
        /// </summary>
        string GetGroupBySql(string[] dims, int[] aggrs)
        {
            var selectColList = new List<string>();
            var groupByList = new List<string>();
            foreach (var dim in dims)
            {
                selectColList.Add(dim);
                groupByList.Add(dim);
            }
            selectColList.Add("COUNT(*) as __Count");
            foreach (var aggrIdx in aggrs)
            {
                var aggrCfg = PvtCfg.Aggregators[aggrIdx];
                if (aggrCfg.Name == "Count")  // count is already present
                    continue;
                string aggrSqlTpl = null;
                if (AggregatorSqlTemplates.TryGetValue(aggrCfg.Name, out aggrSqlTpl))
                {
                    var aggrSql = String.Format(aggrSqlTpl, aggrCfg.Params);
                    selectColList.Add(String.Format("({0}) as {1}_{2}",  // default naming convention for aggregator value: {field}_{aggregatorName}
                        aggrSql,
                        aggrCfg.Params != null && aggrCfg.Params.Length > 0 ? aggrCfg.Params[0] : null,
                        aggrCfg.Name));
                }
                else
                {
                    throw new ArgumentException("Unknown aggregator name: " + aggrCfg.Name);
                }
            }
            return String.Format(
                "SELECT {0} FROM ({1}) as t GROUP BY {2}",
                String.Join(",", selectColList.ToArray()),
                SelectSql,
                String.Join(",", groupByList.ToArray())
            );
        }

        public IPivotData LoadPivotData(string[] dims, int[] aggrs)
        {
            var dbCmd = Connection.CreateCommand();
            dbCmd.CommandText = GetGroupBySql(dims, aggrs);
            var dbCmdSource = new DbCommandSource(dbCmd);

            var groupedPvtDataReader = new GroupedSourceReader(
                    dbCmdSource,
                    "__Count"  // column name with rows count for each entry
                );
            var pvtDataFromGroupBy = groupedPvtDataReader.Read(
                new PivotDataConfiguration()
                {
                    Dimensions = dims,
                    Aggregators = aggrs.Select(aggrIdx => PvtCfg.Aggregators[aggrIdx]).ToArray()
                }, PvtDataFactory);
            return pvtDataFromGroupBy;
        }
    }

}