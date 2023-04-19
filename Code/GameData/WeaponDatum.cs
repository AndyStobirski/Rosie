using Rosie.Code.Misc;

namespace Rosie.Code.GameData
{


    public class WeaponDatum
    {
        public WeaponDatum(WEAPON_TYPE type, WEAPON_SUBTYPE subType, string name, int range, int gFX, int number, int dice, int modifier)
        {
            this.type = type;
            this.subType = subType;
            Name = name;
            Range = range;
            GFX = gFX;
            Dice = dice;
            Modifier = modifier;
            Number = number;
        }

        public WEAPON_TYPE type { get; private set; }
        public WEAPON_SUBTYPE subType { get; private set; }

        public string Name { get; private set; }

        public int Range { get; private set; }

        public int GFX { get; private set; }

        public int Number { get; private set; }

        public int Dice { get; private set; }

        public int Modifier { get; private set; }

    }



}
