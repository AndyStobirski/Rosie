using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Rosie.Misc
{
    /// <summary>
    /// An implementation of the Bresenham's Algorithn
    /// 
    /// https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    /// </summary>
    public static class Bresenhams
    {
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        public static List<Point> BresLine;

        /// <summary>
        /// Delegate function
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public delegate bool PointVisible(int pX, int pY);

        public static bool CanWalkToPoint(int pX, int pY, int pX1, int pY1, PointVisible pFunction)
        {
            return Line(pX, pY, pX1, pY1, pFunction);
        }

        public static bool CanSeeToPoint(int pX, int pY, int pX1, int pY1, PointVisible pFunction)
        {
            return Line(pX, pY, pX1, pY1, pFunction);
        }

        /// <summary>
        /// Plot the line from (x0, y0) to (x1, y1)
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
        public static bool Line(int x0, int y0, int x1, int y1, PointVisible pPointVis)
        {
            BresLine = new List<Point>();

            Point p = new Point();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                p = steep ? new Point(y, x) : new Point(x, y);
                if (pPointVis(p.X, p.Y))
                {
                    BresLine.Add(p);
                    err -= dY;
                    if (err < 0) { y += ystep; err += dX; }
                }
                else
                    return false;
            }

            return true;
        }
    }
}
