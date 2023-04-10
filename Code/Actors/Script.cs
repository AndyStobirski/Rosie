using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Misc;
using Rosie.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rosie.Code.Actors
{
    /// <summary>
    /// An abstract class for monster behaviour which contains primitive functions that can be
    /// chained together in the inherited class to create more complex behaviours
    /// </summary>
    public abstract class Script
    {
        protected NPC monster;
        public Script(NPC pMon)
        {
            monster = pMon;
        }

        /// <summary>
        /// Convenient accessor for Static variable
        /// </summary>
        protected Tile[,] map => RosieGame.currentLevel.Map;

        /// <summary>
        /// Convenient accessor for Static variable
        /// </summary>
        protected Player player => RosieGame.player;

        public abstract void Act();

        /// <summary>
        /// return the point with the lowest manhattan distance relative to the player
        /// </summary>
        protected void DirectMoveTowardsPlayer()
        {
            var p = MapUtils.GetSurroundingPoints(monster.X, monster.Y)
                    .Where(p => MapUtils.IsWalkable(p.X, p.Y))
                    .OrderBy(p => MapUtils.CellDistance(p.X, p.Y, player.X, player.Y))
                    .First();

            monster.Move(p.X, p.Y);
        }

        /// <summary>
        /// Is the player visible via a bresenham's line
        /// </summary>
        /// <returns></returns>
        protected bool CanSeePlayer()
        {
            if (MapUtils.CellDistance(player.X, player.Y, monster.X, monster.Y) <= monster.VisionRange)
            {
                List<Point> bres = null;
                if (LineOfSight(player.X, player.Y, monster.X, monster.Y, ref bres))
                {
                    return true;
                }
            }
            return false;
        }



        protected void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// Plot a brasenham line from (x0, y0) to (x1, y1)
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
        protected bool LineOfSight(int x0, int y0, int x1, int y1, ref List<Point> pLine)
        {
            pLine = new List<Point>();

            Point p = new Point();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                p = steep ? new Point(y, x) : new Point(x, y);
                if (IsSeeThrough(p.X, p.Y))
                {
                    pLine.Add(p);
                    err -= dY;
                    if (err < 0) { y += ystep; err += dX; }
                }
                else
                    return false;
            }

            return true;
        }



        /// <summary>
        /// Can the specified spell be seen through?
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        protected bool IsSeeThrough(int pX, int pY)
        {
            var Map = RosieGame.currentLevel.Map;

            if (pX < 0 || pX >= Map.GetLength(0) || pY < 0 || pY >= Map.GetLength(1))
                return false;

            return Map[pX, pY] != null && Map[pX, pY].SeeThrough();
        }


    }
}
