using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rosie
{

    /// <summary>
    /// Monster navigation algorithm - AStar used to find a route between the two specified points
    /// </summary>
    public class AStar
    {

        static Point[] Directions =
    {
                  new Point(-1, 1),  new Point(0, 1),   new Point(1, 1)
                , new Point(-1, 0),                     new Point(1, 0)
                , new Point(-1, -1), new Point(0, -1),  new Point(1,-1)
            };

        public Tile[,] _map { get; set; }
        public Point _start { get; set; }
        public Point _finish { get; set; }
        public bool Complete { get; set; }

        private HashSet<RouteCell> _exploredTiles;
        private HashSet<RouteCell> Temp;
        private List<RouteCell> Route;


        /// <summary>
        /// Calculate the AStar route tween the two points, and return the first point of that route
        /// </summary>
        /// <param name="map"></param>
        /// <param name="pX">Monster current pos</param>
        /// <param name="pY">Monster current pos</param>
        /// <param name="pX1">Target pos</param>
        /// <param name="pY1">Target pos</param>
        /// <param name="pTargetPoint">The point to move to on successful nav</param>
        /// <returns>Was a route found to the target coords</returns>
        public bool Calculate(Tile[,] map, int pX, int pY, int pX1, int pY1, out Point pTargetPoint)
        {
            pTargetPoint = new Point();

            _map = map;
            _start = new Point(pX, pY);
            _finish = new Point(pX1, pY1); ;
            _exploredTiles = new HashSet<RouteCell>(new PointComparer()) { new RouteCell(_start.X, _start.Y) { Value = 0 } };

            Temp = new HashSet<RouteCell>(new PointComparer());

            bool Found = false;
            bool neighbourCellsFound = false;

            do
            {
                Temp.Clear();
                neighbourCellsFound = false;

                foreach (RouteCell rc in _exploredTiles)
                {

                    foreach (Point dir in Directions)
                    {
                        if (PointValid(rc.X + dir.X, rc.Y + dir.Y))
                        {
                            neighbourCellsFound = true;
                            AddPoint(rc.X + dir.X, rc.Y + dir.Y, rc.Value + 1);

                            if (rc.X == _finish.X && rc.Y == _finish.Y)
                            {
                                Found = true;
                                goto getOutOfHere;
                            }

                        }
                    }

                }

                if (neighbourCellsFound == false)
                {
                    //it is possible that the route attempted is blocked
                    //by the player, environments etc and this will be
                    //indicated by no cells being added to TEMP on a pass
                    //Trace.WriteLine("Route not traversable");
                    return false;
                }

                foreach (var p in Temp)
                    _exploredTiles.Add(p);


                getOutOfHere:;

            }
            while (!Found);

            var move = CalculateRoute();

            pTargetPoint = new Point(move.X, move.Y);

            return true;

        }

        /// <summary>
        /// A route has been found bteween the two points, now process it into a single point
        /// returning the first point of that route
        /// </summary>
        /// <returns></returns>
        private Point CalculateRoute()
        {

            RouteCell current = _exploredTiles.First(rc => rc.X == _finish.X && rc.Y == _finish.Y);

            Route = new List<RouteCell>() { current };

            do
            {

                current = _exploredTiles.First(r =>

                 (
                        (Math.Abs(r.X - current.X) == 1 && Math.Abs(r.Y - current.Y) == 0)
                        || (Math.Abs(r.X - current.X) == 0 && Math.Abs(r.Y - current.Y) == 1)

                    ) && r.Value == (current.Value - 1)

                );

                Route.Add(current);

            } while (current.Value > 0);    //changing this value to 0 includes the start point

            var pt = Route[Route.Count() - 2];

            return new Point(pt.X, pt.Y);

        }

        private void AddPoint(int pX, int pY, int pValue)
        {
            RouteCell r = new RouteCell(pX, pY)
            {
                Value = pValue
            };

            //make sure the point isn't in the temp list
            if (!Temp.Contains(new RouteCell(pX, pY)))
            {
                Temp.Add(r);
            }
        }

        /// <summary>
        /// Check the provided coordinates are valid and belong to a cell that is walkable
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool PointValid(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _map.GetLength(0) && y < _map.GetLength(1))
            {
                if (_map[x, y] != null)
                    if (_map[x, y].Passable())
                    {
                        if (!_exploredTiles.Contains(new RouteCell(x, y)))
                            return true;
                    }

            }
            return false;
        }

        public class RouteCell
        {
            public RouteCell(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }

            /// <summary>
            /// The higher the value, the closer to the finishing point
            /// </summary>
            public int Value { get; set; }

            public override string ToString()
            {
                return $"{X}, {Y}: {Value}";
            }

            public override int GetHashCode()
            {
                // Perfect hash for practical bitmaps, their width/height is never >= 65536
                return (Y << 16) ^ X;
            }
        }

        // inspiration found here https://stackoverflow.com/a/46143745/500181
        class PointComparer : IEqualityComparer<RouteCell>
        {
            public bool Equals(RouteCell x, RouteCell y)
            {
                return x.X == y.X && x.Y == y.Y;
            }

            public int GetHashCode(RouteCell obj)
            {
                // Perfect hash for practical bitmaps, their width/height is never >= 65536
                return (obj.Y << 16) ^ obj.X;
            }
        }
    }
}
