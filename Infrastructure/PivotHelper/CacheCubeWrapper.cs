using NReco.PivotData;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.PivotHelper
{
    /// <summary>
    /// Cache wrapper for better UX on sort/filter.
    /// </summary>
    public class CacheCubeWrapper : ICube
    {

        ICube Cube;

        public string Id
        {
            get { return Cube.Id; }
        }

        public string Name
        {
            get { return Cube.Name; }
        }

        public CacheCubeWrapper(ICube cube)
        {
            Cube = cube;
        }

        public PivotDataConfiguration GetConfiguration()
        {
            var cacheKey = "PivotDataConfiguration:" + Id;
            var pvtCfg = HttpRuntime.Cache.Get(cacheKey) as PivotDataConfiguration;
            if (pvtCfg == null)
            {
                pvtCfg = Cube.GetConfiguration();
                HttpRuntime.Cache[cacheKey] = pvtCfg;
            }
            return pvtCfg;
        }

        public IPivotData LoadPivotData(string[] dims, int[] aggrs)
        {
            var cacheKey = "PivotData:" + Id + ":dims:" + String.Join("#", dims) + ":aggrs:" + String.Join("#", aggrs);
            var pvtData = HttpRuntime.Cache.Get(cacheKey) as IPivotData;
            if (pvtData == null)
            {
                pvtData = Cube.LoadPivotData(dims, aggrs);
                HttpRuntime.Cache[cacheKey] = pvtData;
            }
            return pvtData;
        }
    }
}
