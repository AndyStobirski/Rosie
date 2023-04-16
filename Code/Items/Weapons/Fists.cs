using Rosie.Code.Misc;

namespace Rosie.Code.Items.Weapons
{
    public class Fists : Weapon
    {
        public Fists()
        {
            Damage = new Roll(2, 0);
            Name = "Fist";
            Range = 1;
        }
    }
}
