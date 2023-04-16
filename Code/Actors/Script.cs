using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Misc;
using Rosie.Entities;
using Rosie.Enums;
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
        private static Random rnd = new Random();

        protected static Roll d20 = new Roll(20, 0);


        /// <summary>
        /// The location the monster is trying to get
        /// </summary>
        public WayPoint TargetWayPoint { get; set; }

        /// <summary>
        /// Navigate the dungeon with this algorithm
        /// </summary>
        private AStar aStar;

        /// <summary>
        /// The number of turns the NPC is unable to get it's target and is
        /// stuck in one place
        /// </summary>
        protected int IdleCounter = 0;

        /// <summary>
        /// When this value is exceed, and the NPC is wandering choose a new direction
        /// </summary>
        protected int IdleThreshHold = 3;

        /// <summary>
        /// How long the monser is to sleep for
        /// </summary>
        protected int SleepCounter = 0;

        /// <summary>
        /// Probability will sleep
        /// </summary>
        protected int SleepProb = 10;

        /// <summary>
        /// How long does it sleep foor
        /// </summary>
        protected int SleepCount { get; set; }


        /// <summary>
        /// Current behaviour state
        /// </summary>
        public NPC_STATE State { get; set; }

        protected NPC monster;

        public void setMonster(NPC pNPC)
        {
            monster = pNPC;
        }

        public Script()
        {
            aStar = new AStar(RosieGame.currentLevel.Map);
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
        protected void DirectMoveTowardsPoint(int pX, int pY)
        {
            var p = MapUtils.GetSurroundingPoints(monster.X, monster.Y)
                    .Where(p => MapUtils.IsWalkable(p.X, p.Y))
                    .OrderBy(p => MapUtils.CellDistance(p.X, p.Y, pX, pY))
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
                    State = NPC_STATE.Alert;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set a new TargetWayPoint based on the current target. Each target contains
        /// a list of connected Waypoints, so randomly choose one.
        /// </summary>
        /// <param name="pWayPoint"></param>
        public void SetTargetWayPoint(WayPoint pWayPoint)
        {
            TargetWayPoint = pWayPoint.ConnectedPoints[rnd.Next(pWayPoint.ConnectedPoints.Count)];
            RosieGame.AddMessage("Monster Target Waypoint {0}", TargetWayPoint.ToString());
        }

        /// <summary>
        /// Simple wander - try to get to the current defined TargetWaypoint. If the target can't be reached 
        /// </summary>
        public void Wander()
        {

            if ((monster.X == TargetWayPoint.X && monster.Y == TargetWayPoint.Y))
            {
                SetTargetWayPoint(TargetWayPoint);
            }
            State = NPC_STATE.Exploring;


            Point p;
            if (aStar.Calculate(monster.X, monster.Y, TargetWayPoint.X, TargetWayPoint.Y, out p))
            {
                monster.Move(p.X, p.Y);
                IdleCounter = 0;
            }
            else
            {
                if (IdleCounter++ > IdleThreshHold)
                {
                    IdleCounter = 0;
                    SetTargetWayPoint(TargetWayPoint);
                    RosieGame.AddMessage("Monster stuck choosing a new wander location");
                }
            }
        }

        protected void SetSleep()
        {

            SleepCount = 25;
            State = NPC_STATE.Sleeping;
            RosieGame.AddMessage("Monster feel asleep");
        }

        protected void Sleep()
        {
            SleepCount--;

            if (SleepCount <= 0)
            {
                State = NPC_STATE.Idle;
                RosieGame.AddMessage("Monster woke up");
            }
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
