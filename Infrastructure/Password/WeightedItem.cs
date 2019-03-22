using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Password
{
    public static class WeightedItem
    {
        public static WeightedItem<T> Create<T>(uint weight, T item)
        {
            return new WeightedItem<T>
            {
                Item = item,
                Weight = weight,
            };
        }
    }

    [DataContract]
    public class WeightedItem<T>
    {
        [DataMember]
        public uint Weight { get; set; }

        [DataMember]
        public T Item { get; set; }
    }
}
