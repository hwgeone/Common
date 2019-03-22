using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Password
{
    [DataContract]
    public class SerializableTrigramStatistics
    {
        [DataMember]
        public List<WeightedItem<Tuple<char, char>>> PrefixWeights { get; set; }

        [DataMember]
        public Dictionary<Tuple<char, char>, List<WeightedItem<char>>> TrigramWeights { get; set; }
    }
}
