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
    /// Cube for serialized PivotData file. 
    /// </summary>
    public class PivotDataFileCube : ICube
    {

        string CubeFileName;

        public string Id { get { return CubeFileName; } }

        public string Name { get; set; }

        public PivotDataFileCube(string cubeFileName)
        {
            CubeFileName = cubeFileName;
            Name = cubeFileName;
        }

        IPivotData LoadCubeFromFile()
        {
            var cubePath = HttpContext.Current.Server.MapPath(Path.Combine("~/App_Data", CubeFileName));
            var cubeRdr = new CubeFileReader(cubePath);
            return cubeRdr.Read();
        }

        public PivotDataConfiguration GetConfiguration()
        {
            return new PivotDataFactory().GetConfiguration(LoadCubeFromFile());
        }

        public IPivotData LoadPivotData(string[] dims, int[] aggrs)
        {
            var savedCube = LoadCubeFromFile();
            var sliceQuery = new SliceQuery(savedCube);
            foreach (var dim in dims)
                sliceQuery.Dimension(dim);
            foreach (var aggrIdx in aggrs)
                sliceQuery.Measure(aggrIdx);
            return sliceQuery.Execute();
        }
    }

}