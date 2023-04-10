using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosie.Code.Environment
{
    public class Staircase : Tile
    {
        public bool IsUp { get; set; }

        public Staircase(bool pIsUp) 
        {
            IsUp = pIsUp;
            Gfx  = (int)(IsUp ? GFXValues.STAIRCASE_UP : GFXValues.STAIRCASE_DOWN);
        }

        public override bool Passable()
        {
            throw new NotImplementedException();
        }

        public override bool SeeThrough()
        {
            throw new NotImplementedException();
        }
    }
}
