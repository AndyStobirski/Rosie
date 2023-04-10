using Rosie.Code.Misc;
using System;
using System.Linq;

namespace Rosie.Misc
{
    public class Roller
    {

        static Random _rnd = new Random();

        public static int Roll(Roll pDice)
        {
            return Enumerable.Range(1, pDice.Multiplier)
                        .Select(i => _rnd.Next(1, pDice.Dice + 1)).Sum()
                        + pDice.Modifier;
        }
    }
}
