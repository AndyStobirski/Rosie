using Rosie.Code.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosie.Code.Items
{
    public class Armour : Item
    {
        public Roll Defence { get; protected set; }

        public Armour()
        {

        }

        public Armour(int pGfx) : base(pGfx)
        {
            pGfx = (int)GFXValues.ARMOUR_LEATHER;
        }
    }
}
