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
    /// Implements data cube for SQL data that performs aggregation with PivotData.ProcessData.
    /// </summary>
    public class SqlCube : ICube
    {

        public string Id { get; private set; }

        public string Name { get; set; }

        IDbConnection Connection;
        string SelectSql;
        PivotDataConfiguration PvtCfg;

        public PivotDataFactory PvtDataFactory { get; set; }

        /// <summary>
        /// Initializes <see cref="SqlCube"/> instance.
        /// </summary>
        /// <param name="id">cube name (unique identifier)</param>
        /// <param name="pvtCfg">cube configuration: available dimensions and measures</param>
        /// <param name="connection">ADO.NET database connection instance</param>
        /// <param name="selectSql">select that returns all columns that may be used as dimensions or as measures</param>
        public SqlCube(string id, PivotDataConfiguration pvtCfg, IDbConnection connection, string selectSql)
        {
            Id = id;
            Name = id;
            PvtCfg = pvtCfg;
            Connection = connection;
            SelectSql = selectSql;

            PvtDataFactory = new PivotDataFactory();
        }

        public PivotDataConfiguration GetConfiguration()
        {
            return PvtCfg;
        }

        /// <summary>
        /// This method composes SQL command only for columns needed by specified dimensions/aggregators
        /// </summary>
        string GetSql(string[] dims, int[] aggrs)
        {
            var selectColList = new List<string>();
            foreach (var dim in dims)
            {
                selectColList.Add(dim);
            }
            foreach (var aggrIdx in aggrs)
            {
                var aggrCfg = PvtCfg.Aggregators[aggrIdx];
                if (aggrCfg.Name == "Count")  // nothing else is needed for "count"
                    continue;
                if (aggrCfg.Params != null)
                    foreach (var aggrParam in aggrCfg.Params)
                        if (aggrParam is string)
                            selectColList.Add((string)aggrParam);
            }
            return String.Format(
                "SELECT {0} FROM ({1}) as t",
                String.Join(",", selectColList.Distinct().ToArray()),
                SelectSql);
        }

        public IPivotData LoadPivotData(string[] dims, int[] aggrs)
        {
            var dbCmd = Connection.CreateCommand();
            dbCmd.CommandText = GetSql(dims, aggrs);
            var dbCmdSource = new DbCommandSource(dbCmd);

            var pvtData = PvtDataFactory.Create(
                new PivotDataConfiguration()
                {
                    Dimensions = dims,
                    Aggregators = aggrs.Select(aggrIdx => PvtCfg.Aggregators[aggrIdx]).ToArray()
                });
            pvtData.ProcessData(dbCmdSource);
            return pvtData;
        }
    }

}