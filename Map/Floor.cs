namespace Rosie.Map
{
    public class Floor : Tile
    {
        public Floor():base(0)
        {
        }

        public Floor(int pGfx):base (pGfx)
        {

        }

        public override bool Walkable()
        {

            if (
                    Inhabitant == null && 
                    Solid == 0)
                return true;
            return false;
        }
    }
}
