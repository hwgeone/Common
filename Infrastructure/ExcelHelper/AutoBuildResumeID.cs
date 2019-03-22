using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExcelHelper
{
    public static class AutoBuildResumeID
    {
        public static string BulidResumeID()
        {
            string time = DateTime.Now.ToString("yyMMdd");
            string guid = Guid.NewGuid().ToString().GetHashCode().ToString("x");
            return time + guid;
        }
    }
}
