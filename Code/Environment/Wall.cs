﻿using Rosie.Code.Misc;

namespace Rosie.Code.Environment
{
    public class Wall : Tile
    {

        public Wall() : base(0)
        {
            this.Gfx = (int)GFXValues.DUNGEON_WALL;
            this.Solid = 1;
        }

        public override bool Passable()
        {
            return false;
        }

        public override bool SeeThrough()
        {
            return false;
        }

        public override string Description()
        {
            return MessageStrings.See_YouSee + " a wall";
        }
    }
}
