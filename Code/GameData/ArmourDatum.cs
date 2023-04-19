using Rosie.Code.Misc;

namespace Rosie.Code.GameData
{
    public class ArmourDatum
    {
        public ArmourDatum(string name, int gFX, Armour_Type type, Armour_SubType subType, int number, int dice, int modifier)
        {
            Dice = dice;
            Modifier = modifier;
            Number = number;
            Name = name;
            GFX = gFX;
            Type = type;
            SubType = subType;
        }

        public int Number { get; private set; }

        public int Dice { get; private set; }

        public int Modifier { get; private set; }
        public string Name { get; private set; }
        public int GFX { get; private set; }
        public Armour_Type Type { get; private set; }
        public Armour_SubType SubType { get; private set; }

    }


}
