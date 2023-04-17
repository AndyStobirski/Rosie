using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rosie.Code.Misc;

namespace Rosie.Code.Items
{
    internal class LeatherArmour : Armour
    {
        public LeatherArmour()
        {
            Name = "Leather armour";
            Gfx = (int)GFXValues.ARMOUR_LEATHER;
            Defence = new Misc.Roll(1, 4, 0);
        }
    }
}
