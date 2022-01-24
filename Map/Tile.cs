using Rosie.Entities;

namespace Rosie.Map
{
    public abstract class Tile
    {
        public Tile(int gfx)
        {
            Gfx = gfx;
        }

        public abstract bool Walkable();

        public Actor Inhabitant { get; set; }

        public int Gfx { get; set; }
        public int Viewed { get; set; } 
        public int Solid { get; set; }

        public override string ToString()
        {
            return $"{0},{1}:{Inhabitant == null}";
        }

    }
}
