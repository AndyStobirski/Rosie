using Rosie.Code.Misc;
using System.Linq;

namespace Rosie.Misc
{
    public class Roller
    {

        public static int Roll(NDM pDice)
        {
            return Enumerable.Range(0, pDice.Number + 1)
                        .Select(i => RandomWithSeed.Next(1, pDice.Dice + 1)).Sum()
                        + pDice.Modifier;
        }
    }
}
