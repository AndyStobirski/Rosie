namespace Rosie.Code.Environment
{
    public class Floor : Tile
    {
        public Floor() : base(0)
        {
            Gfx = (int)GFXValues.DUNGEON_FLOOR;
        }

        public Floor(int pGfx) : base(pGfx)
        {

        }

        public override bool SeeThrough()
        {
            return Solid == 0;
        }

        public override bool Passable()
        {

            if (
                    Inhabitant == null &&
                    Solid == 0)
                return true;
            return false;
        }
    }
}
