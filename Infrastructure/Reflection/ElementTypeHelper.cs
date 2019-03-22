using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reflection
{
    public static class ElementTypeHelper
    {
        public static Type GetEnumerationType(Type enumType)
        {
            return !enumType.IsEnum() ? null : enumType;
        }
    }
}
