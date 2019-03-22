using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using NReco.PivotData;
using NReco.PivotData.Input;

namespace Services
{

    /// <summary>
    /// Cube for in-memory IPivotData instance.
    /// </summary>
    public class InMemoryCube : ICube
    {

        IPivotData PvtData;

        public string Id { get; set; }

        public string Name { get; set; }

        public InMemoryCube(string id, IPivotData pvtData)
        {
            PvtData = pvtData;
            Id = id;
            Name = id;
        }

        public PivotDataConfiguration GetConfiguration()
        {
            // cube configuration is taken from actual IPivotData instance setup
            return new PivotDataFactory().GetConfiguration(PvtData);
        }

        public IPivotData LoadPivotData(string[] dims, int[] aggrs)
        {
            var sliceQuery = new SliceQuery(PvtData);
            foreach (var dim in dims)
                sliceQuery.Dimension(dim);
            foreach (var aggrIdx in aggrs)
                sliceQuery.Measure(aggrIdx);
            return sliceQuery.Execute();
        }
    }

}