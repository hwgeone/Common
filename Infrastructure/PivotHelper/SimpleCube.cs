using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using NReco.PivotData;
using NReco.PivotData.Input;
using Services;
using System.Data;

namespace Infrastructure.PivotHelper
{
    public class SimpleCube : ICube
    {
        public string Id { get; private set; }

        public string Name { get; set; }
        public DataTable SourceData { get; set; }
        public PivotDataFactory PvtDataFactory { get; set; }
        PivotDataConfiguration PvtCfg;

        public SimpleCube(string id, DataTable data, PivotDataConfiguration pvtCfg)
        {
            Id = id;
            Name = id;
            SourceData = data;
            PvtCfg = pvtCfg;
            PvtDataFactory = new PivotDataFactory();
        }

        public PivotDataConfiguration GetConfiguration()
        {
            return PvtCfg;
        }

        public IPivotData LoadPivotData(string[] dims, int[] aggrs)
        {
            try
            {
                var pvtData = PvtDataFactory.Create(
                    new PivotDataConfiguration()
                    {
                        Dimensions = dims,
                        Aggregators = aggrs.Select(aggrIdx => PvtCfg.Aggregators[aggrIdx]).ToArray()
                    });
                pvtData.ProcessData(new DataTableReader(SourceData));
                return pvtData;
            }
            catch (Exception ex)
            {
 
            }

            return null;
        }
    }
}
