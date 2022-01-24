using Microsoft.Xna.Framework;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rosie.Map
{
    public class CustomGenerator : Generator
    {
        public override CurrentLevel Build()
        {
            Level = new CurrentLevel();

            MapSize = new Size(30, 30);
            Level.Map = new Tile[MapSize.Width, MapSize.Height];

            for(int x = 5; x < 25; x++)
            {
                Level.Map[x, 15] = new Floor(); //vertical line
                Level.Map[15, x] = new Floor(); //horizotal line
            }

            for (int y = 13; y < 18; y++)
                for (int x = 13; x < 18; x++)
            {
                    Level.Map[y, x] = new Floor();
                }

            return Level;

        }

        public override Point GetStartLocation()
        {
            return new Point(15, 15);
        }
    }
}
