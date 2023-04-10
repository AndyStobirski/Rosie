using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosie.Code.Items
{
    public class GoldCoins : Item
    {
        public int Value { get; private set; }

        public GoldCoins(int pValue) 
        {
            Gfx = (int)GFXValues.TREASURE_GOLDPIECE;
            Value = pValue;
            Name = "Gold Coins";
        }
    }
}
