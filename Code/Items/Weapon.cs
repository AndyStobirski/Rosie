using Rosie.Code.GameData;
using Rosie.Code.Misc;

namespace Rosie.Code.Items
{
    public class Weapon : Item
    {

        public Weapon(WeaponDatum datum)
        {
            Name = datum.Name;
            Range = datum.Range;
            Gfx = datum.GFX;
            Damage = new NDM(datum.Number, datum.Dice, datum.Modifier);
            type = datum.type;
            subType = datum.subType;

        }

        public WEAPON_TYPE type { get; private set; }
        public WEAPON_SUBTYPE subType { get; private set; }


        public int Range { get; private set; }

        public NDM Damage { get; private set; }
    }
}
