
using Rosie.Code.GameData;
using Rosie.Code.Misc;

namespace Rosie.Code.Items
{
    public class Armour : Item
    {


        public Armour(ArmourDatum datum)
        {
            Defence = new NDM(datum.Number, datum.Dice, datum.Modifier);
            Name = datum.Name;
            Gfx = datum.GFX;
            Type = datum.Type;
            SubType = datum.SubType;

        }

        public NDM Defence { get; private set; }

        public Armour_Type Type { get; private set; }
        public Armour_SubType SubType { get; private set; }
    }
}
