using Rosie.Code.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rosie.Code.Items.Weapons
{
    public class Dagger : Weapon
    {
        public Dagger()
        {
            Damage = new Roll(1, 4, 0);
            Name = "Dagger";
            Range = 1;
            Gfx = (int)GFXValues.WEAPON_DAGGER;
        }
    }
}
