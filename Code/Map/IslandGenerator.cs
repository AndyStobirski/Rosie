using Microsoft.Xna.Framework;
using Rosie.Misc;
using System;
using Rosie.Code.Environment;
using Rosie.Code.Map;

namespace Rosie.Map
{

    /// <summary>
    /// IslandMaze class - generates simple islands and mazes.
    /// 
    /// For more info on it's use see http://www.evilscience.co.uk/?p=53
    /// </summary> 
    class IslandGenerator : Generator
    {
        public override Level Build()
        {
            Level = new Level
            {
                Map = new Tile[MapSize.Width, MapSize.Height]
            };


            //go through each cell and use the specified probability to determine if it's open
            for (int x = 0; x < Level.Map.GetLength(0); x++)
            {
                for (int y = 0; y < Level.Map.GetLength(1); y++)
                {
                    if (_rnd.Next(0, 100) < CloseCellProb)
                    {
                        Level.Map[x, y] = new Floor();
                    }
                }
            }

            //pick some cells at random
            for (int x = 0; x <= Iterations; x++)
            {
                int rX = _rnd.Next(0, Level.Map.GetLength(0));
                int rY = _rnd.Next(0, Level.Map.GetLength(1));

                if (ProbExceeded == true)
                {
                    if (examineNeighbours(rX, rY) > Neighbours)
                    {
                        Level.Map[rX, rY] = new Floor();
                    }
                    else
                    {
                        Level.Map[rX, rY] = null;
                    }
                }
                else
                {
                    if (examineNeighbours(rX, rY) > Neighbours)
                    {
                        Level.Map[rX, rY] = null;
                    }
                    else
                    {
                        Level.Map[rX, rY] = new Floor();
                    }
                }
            }

            return Level;

        }




        public int Neighbours { get; set; }
        public int Iterations { get; set; }
        public int CloseCellProb { get; set; }
        public bool ProbExceeded { get; set; }

        public IslandGenerator()
        {
            Neighbours = 4;
            Iterations = 50000;
            ProbExceeded = true;
            MapSize = new Size(99, 99);
            CloseCellProb = 45;

        }


        /// <summary>
        /// Count all the closed cells around the specified cell and return that number
        /// </summary>
        /// <param name="xVal">cell X value</param>
        /// <param name="yVal">cell Y value</param>
        /// <returns>Number of surrounding cells</returns>
        private int examineNeighbours(int xVal, int yVal)
        {
            int count = 0;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (checkCell(xVal + x, yVal + y) == true)
                        count += 1;
                }
            }

            return count;
        }

        /// <summary>
        /// Check the examined cell is legal and closed
        /// </summary>
        /// <param name="x">cell X value</param>
        /// <param name="y">cell Y value</param>
        /// <returns>Cell state - true if closed, false if open or illegal</returns>
        private Boolean checkCell(int x, int y)
        {
            if (x >= 0 & x < Level.Map.GetLength(0) &
                y >= 0 & y < Level.Map.GetLength(1))
            {
                if (Level.Map[x, y] != null)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public override Point GetStartLocation()
        {
            int x;
            int y;

            do
            {
                x = _rnd.Next(0, MapSize.Width);
                y = _rnd.Next(0, MapSize.Height);

            } while (Level.Map[x, y] == null);

            return new Point(x, y);
        }

    }

}