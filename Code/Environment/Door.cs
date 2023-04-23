using Rosie.Code.Interfaces;
using Rosie.Code.Misc;

namespace Rosie.Code.Environment
{
    public class Door : Tile, iOpenable
    {
        private int gfxOpen { get; set; } = (int)GFXValues.DUNGEON_DOOR_OPEN;
        private int gfxClose { get; set; } = (int)GFXValues.DUNGEON_DOOR_CLOSE;


        public Door() : base(0)
        {
            IsOpen = true;

        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                Gfx = IsOpen ? gfxClose : gfxOpen;
            }
        }

        public override string Description()
        {
            return MessageStrings.See_YouSee + " an " + (IsOpen ? "open" : "closed") + " door";
        }

        public override bool SeeThrough()
        {
            return IsOpen;
        }

        public override bool Passable()
        {
            return IsOpen;
        }

        public bool Open()
        {
            IsOpen = true;
            return true;
        }

        public bool Close()
        {
            IsOpen = false;
            return true;
        }
    }
}
