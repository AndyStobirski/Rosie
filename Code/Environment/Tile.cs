using Rosie.Code.Items;
using Rosie.Code.sensedata;
using Rosie.Entities;
using System.Collections.Generic;

namespace Rosie.Code.Environment
{
    public abstract class Tile
    {
        public Tile(int gfx)
        {
            Gfx = gfx;
        }

        public Tile()
        {

        }

        public abstract string Description();

        /// <summary>
        /// Does the tile allow an actor to move over it?
        /// </summary>
        /// <returns></returns>
        public abstract bool Passable();

        /// <summary>
        /// Can the tile be seen through? Doesn't have to be passable to be see through.
        /// </summary>
        /// <returns></returns>
        public abstract bool SeeThrough();

        public Actor Inhabitant { get; set; }

        public List<Item> Items = new List<Item>();

        public List<SenseDatum> SenseData = new List<SenseDatum>();

        /// <summary>
        /// The index of the item on the tile sheet
        /// </summary>
        public virtual int Gfx { get; set; }
        /// <summary>
        /// Has this tile been seen by the player
        /// </summary>
        public int Viewed { get; set; }

        /// <summary>
        /// Is it completely filled?
        /// </summary>
        public int Solid { get; set; }

        public override string ToString()
        {
            return $"{0},{1}:{Inhabitant == null}";
        }


    }
}
