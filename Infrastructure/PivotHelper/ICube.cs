using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NReco.PivotData;

namespace Services
{

    public interface ICube
    {

        string Id { get; }

        string Name { get; }

        PivotDataConfiguration GetConfiguration();

        IPivotData LoadPivotData(string[] dims, int[] aggrs);
    }
}
