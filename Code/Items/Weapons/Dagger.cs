using Rosie.Code.Misc;

namespace Rosie.Code.Items.Weapons
{
    public class Dagger : Weapon
    {
        public Dagger()
        {
            Damage = new Roll(4, 0);
            Name = "Dagger";
            Range = 1;
            Gfx = (int)GFXValues.WEAPON_DAGGER;
        }
    }
}
