using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Password
{
    public static class Sets
    {
        public const string Lower = "abcdefghkmnpqrty";
        public const string Upper = "ABCDEFGHKMNPQRTY";
        public const string Digits = "2345678";
        public const string Alphanumerics = Lower + Upper + Digits;

        public static IEnumerable<string> AlphanumericGroups
        {
            get { return new[] { Lower, Upper, Digits }; }
        }

        public const string Symbols = "!@$%&*()";
        public const string FullSymbols = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";
    }
}
