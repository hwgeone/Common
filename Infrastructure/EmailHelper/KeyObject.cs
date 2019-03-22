using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailHelper
{
    public class KeyObject:Object
    {
        public const string Split = "@"; 
        public string DisplayName { get; set; }
        public string PropertyName { get; set; }

        public KeyObject(string propertyName,string displayName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            DisplayName = displayName;
            PropertyName = propertyName;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return PropertyName;
            return DisplayName + Split + PropertyName;
        }
    }
}
