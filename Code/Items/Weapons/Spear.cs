using Rosie.Code.Misc;

namespace Rosie.Code.Items.Weapons
{
    public class Spear : Weapon
    {
        public Spear()
        {
            Damage = new Roll(4, 0);
            Name = "Dagger";
            Range = 2;
            Gfx = (int)GFXValues.WEAPON_SPEAR;
        }
    }
}
