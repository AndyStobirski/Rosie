using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rosie.Misc
{
    public class Roller
    {
        Random _rnd = new Random();
        Regex _reg = new Regex(@"(?<mult>\d+)D(?<die>\d+)(?<mod>[-+]\d+)?", RegexOptions.IgnoreCase);

        public int Roll(string pValue)
        {

            Match m = _reg.Match(pValue);
            int mult = Convert.ToInt32(m.Groups["mult"].ToString());
            int die = Convert.ToInt32(m.Groups["die"].ToString()) + 1; //add one,as the upper value is 1 less than the max
            int mod = string.IsNullOrEmpty(m.Groups["die"].ToString()) ? 0 : Convert.ToInt32(m.Groups["die"].ToString());

            return Enumerable.Range(0, mult).Select(i => _rnd.Next(1, die)).Sum() + mod;
        }

    }
}
