using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NReco.PivotData;

namespace Privot.Models
{
    public class PivotTableReportConfig
    {
        public string CubeName { get; set; }

        public Dimension[] Rows { get; set; }
        public Dimension[] Columns { get; set; }
        public Measure[] Measures { get; set; }

        public bool SubtotalColumns { get; set; }
        public bool SubtotalRows { get; set; }

        public bool GrandTotal { get; set; }
        public bool TotalsRow { get; set; }
        public bool TotalsColumn { get; set; }

        public int? LimitRows { get; set; }
        public int? LimitColumns { get; set; }

        public OrderOptions OrderBy { get; set; }

        public string Filter { get; set; }

        public PaginatePivotTable.Page ColumnPage { get; set; }
        public PaginatePivotTable.Page RowPage { get; set; }


        public PivotTableConfiguration GetPivotTableConfig(PivotDataConfiguration cubeSchema)
        {
            var pvtTblCfg = new PivotTableConfiguration();

            pvtTblCfg.Columns = GetDimensionNames(cubeSchema, Columns, "Columns");
            pvtTblCfg.Rows = GetDimensionNames(cubeSchema, Rows, "Rows");
            pvtTblCfg.Measures = Measures != null ? GetMeasureIndexes(cubeSchema, Measures) : new int[] { 0 };

            if (OrderBy != null)
            {
                pvtTblCfg.PreserveGroupOrder = OrderBy.PreserveGroupOrder;
                pvtTblCfg.OrderKeys = OrderBy.Dimensions;
                pvtTblCfg.SortByValue = OrderBy.Values;
            }

            return pvtTblCfg;
        }

        string[] GetDimensionNames(PivotDataConfiguration cubeSchema, Dimension[] dims, string axis)
        {
            var res = new string[dims != null ? dims.Length : 0];
            for (int i = 0; i < res.Length; i++)
            {
                var dimName = dims[i].Name;
                if (!cubeSchema.Dimensions.Where(d => d == dimName).Any())
                    throw new ArgumentException(String.Format("{0} dimension does not exist: '{1}'", axis, dimName));
                res[i] = dimName;
            }
            return res;
        }

        int[] GetMeasureIndexes(PivotDataConfiguration cubeSchema, Measure[] measures)
        {
            var res = new int[measures.Length];
            for (int i = 0; i < res.Length; i++)
            {
                for (int j = 0; j < cubeSchema.Aggregators.Length; j++)
                    if (j.ToString() == measures[i].Name)
                        res[i] = j;
            }
            return res;
        }


        public class Dimension
        {
            public string Name { get; set; }
        }

        public class Measure
        {
            public string Name { get; set; }
            public PercentagePivotTable.PercentageMode? Percentage { get; set; }
            public DifferencePivotTable.DifferenceMode? Difference { get; set; }
            public RunningValuePivotTable.Direction? RunningTotal { get; set; }
            public bool DifferenceAsPercentage { get; set; }
            public HeatmapPivotTable.HeatmapMode? Heatmap { get; set; }
        }

        public class OrderOptions
        {
            public PivotTableConfiguration.AxisKeysOrder[] Dimensions { get; set; }
            public PivotTableConfiguration.AxisValuesOrder Values { get; set; }

            /// <summary>
            /// Preserves grouping order when sort by value is specified.
            /// </summary>
            public bool PreserveGroupOrder { get; set; }
        }
    }

}