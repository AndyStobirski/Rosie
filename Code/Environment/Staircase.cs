using Rosie.Code.Misc;

namespace Rosie.Code.Environment
{
    public class Staircase : Tile
    {
        public bool IsUp { get; set; }

        public Staircase(bool pIsUp)
        {
            IsUp = pIsUp;
            Gfx = (int)(IsUp ? GFXValues.STAIRCASE_UP : GFXValues.STAIRCASE_DOWN);

        }


        public override bool Passable()
        {
            return true;
        }

        public override bool SeeThrough()
        {
            return true;
        }
    }
}
