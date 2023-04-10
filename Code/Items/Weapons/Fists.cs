using Rosie.Code.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rosie.Code.Items.Weapons
{
    public class Fists : Weapon
    {
        public Fists()
        {
            Damage = new Roll(1, 2, 0);
            Name = "Fist";
            Range = 1;
        }
    }
}
