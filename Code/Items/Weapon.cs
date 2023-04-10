using Rosie.Code.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rosie.Code.Items
{
    public class Weapon : Item
    {
        public Roll Damage { get; set; }

        public int Range { get; set; }

        public Weapon()
        {

        }

        public Weapon(int pGfx) : base(pGfx)
        {

        }
    }
}
