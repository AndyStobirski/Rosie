using Rosie.Code.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rosie.Code.Items.Weapons
{
    public class Spear : Weapon
    {
        public Spear()
        {
            Damage = new Roll(1, 4, 0);
            Name = "Dagger";
            Range = 2;
            Gfx = (int)GFXValues.WEAPON_SPEAR;
        }
    }
}
