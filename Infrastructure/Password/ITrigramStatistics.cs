using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Password
{
    public interface ITrigramStatistics
    {
        IReadOnlyCollection<WeightedItem<Tuple<char, char>>> GetPrefixWeights();
        IReadOnlyCollection<WeightedItem<char>> GetTrigramWeights(Tuple<char, char> prefix);
        bool Exists(Tuple<char, char> prefix);
    }
}
