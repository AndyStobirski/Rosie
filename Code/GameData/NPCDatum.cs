using Rosie.Code.Misc;

namespace Rosie.Code.GameData
{
    /// <summary>
    /// Used to define an NPC, passed into the class constructor
    /// </summary>
    public class NPCDatum
    {
        public NPCDatum(NPC_Type type, NPC_SubType substype, string name, int speed, int maxHitPoint, string script, int gfx, int XP)
        {
            Type = type;
            SubType = substype;
            Name = name;
            Speed = speed;
            MaxHitPoint = maxHitPoint;
            Script = script;
            Gfx = gfx;
            ExperienceValue = XP;
        }

        public NPC_SubType SubType { get; private set; }
        public NPC_Type Type { get; private set; }
        public string Name { get; private set; }
        public int Speed { get; private set; }
        public int MaxHitPoint { get; private set; }
        public string Script { get; private set; }
        public int Gfx { get; private set; }
        public int ExperienceValue { get; private set; }

    }
}
