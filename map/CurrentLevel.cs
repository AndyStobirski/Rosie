using Microsoft.Xna.Framework;
using Rosie.Entities;
using Rosie.Enums;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Rosie.Map
{
    public class CurrentLevel
    {
        Scheduler _Scheduler = new Scheduler();

        IEnumerable<Actor> actors;

        /// <summary>
        /// Monsters that live on the current level
        /// </summary>
        public List<Monster> Monsters = new List<Monster>();

        public Player player { get; set; }

        public Tile[,] Map { get; set; }

        public GameStates GameState { get; set; }

        protected Random _rnd = new Random();

        private AStar _astar = new AStar();

        /// <summary>
        /// Used for monster navigation
        /// </summary>
        public List<WayPoint> wayPoints = new List<WayPoint>();

        public void DoMonster(Monster pMonster)
        {

            if (pMonster.AtRoamTarget())
            {
                pMonster.RoamTarget = GetRandomWayPoint(pMonster.RoamTarget);

            }
            else
            {
                Point move = new Point();
                if (_astar.Calculate(Map, pMonster.X, pMonster.Y, pMonster.RoamTarget.X, pMonster.RoamTarget.Y, out move))
                {
                    pMonster.Move(move.X, move.Y);
                }
                else
                {
                    //we're here because the route can be walked
                    //so get a new point
                    pMonster.RoamTarget = GetRandomWayPoint(pMonster.RoamTarget);
                }
            }

        }

        public void M_ActorMoved(object sender, Actor.ActorMovedEventArgs e)
        {
            Map[e.Before.X, e.Before.Y].Inhabitant = null;
            Map[e.After.X, e.After.Y].Inhabitant = e.Inhabitant;
        }


        /// <summary>
        /// Write the current map to a text file as a 2d integer array
        /// </summary>
        public void OutputMap()
        {
            using (StreamWriter f = new StreamWriter($"{Guid.NewGuid().ToString()}.txt"))
            {

                f.WriteLine("{");

                for (int y = 0; y < Map.GetLength(0); y++)
                {

                    f.Write("\t{");

                    for (int x = 0; x < Map.GetLength(1); x++)
                    {
                        f.Write((Map[y, x] == null ? "1" : "0") + (x < Map.GetLength(1) - 1 ? ", " : ""));
                    }

                    f.WriteLine("\t}" + (y < Map.GetLength(0) - 1 ? ", " : ""));
                }

                f.WriteLine("}");
            }

        }

        /// <summary>
        /// Get a random waypoint
        /// </summary>
        /// <param name="pExclude">Point to exclude from random list</param>
        /// <returns>New waypoint</returns>
        private WayPoint GetRandomWayPoint(WayPoint pExclude)
        {
            var w = wayPoints.Where(w => w != pExclude).ToArray();
            return w[_rnd.Next(0, w.Count())];
        }



        public void Tick()
        {

            actors = _Scheduler.Tick();

            if (actors.Contains(player))
                GameState = GameStates.PlayerTurn;
            else
                GameState = GameStates.EnemyTurn;

            foreach (var act in actors)
            {
                if (act is Monster)
                    (act as Monster).Act();
            }


        }

        /// <summary>
        /// Set up monsters on the lebels
        /// </summary>
        public void InitMonsters(Player pPlayer, int pMaxMonsters)
        {

            player = pPlayer;
            _Scheduler.AddActor(pPlayer);

            Monster m;

            for (int ctr = 0; ctr < pMaxMonsters; ctr++)
            {
                m = new Monster(RandomWalkableCell())
                {
                    Gfx = _rnd.Next(10, 100)
                };
                m.ActorMoved += M_ActorMoved;
                m.BehaviourChange += M_BehaviourChange;
                m.Speed = 2;

                m.Behaviour = MonsterBehaviour.Hunting;



                Console.WriteLine("Monster {0},{1}", m.RoamTarget.X, m.RoamTarget.Y);

                Map[m.X, m.Y].Inhabitant = m;
                Monsters.Add(m);
                _Scheduler.AddActor(m);
            }
        }

        private void M_BehaviourChange(object sender, EventArgs e)
        {
            Monster m = sender as Monster;

            switch (m.Behaviour)
            {
                case MonsterBehaviour.Fleeing:
                    break;

                case MonsterBehaviour.Hunting:
                    m.RoamTarget = GetRandomWayPoint(m.RoamTarget);
                    break;

                case MonsterBehaviour.Passive:
                    break;

                case MonsterBehaviour.Sleeping:
                    break;

                case MonsterBehaviour.Wandering:
                    break;
            }
        }


        /// <summary>
        /// Return a random walkable cell from the map
        /// </summary>
        /// <returns></returns>
        public Point RandomWalkableCell()
        {
            int x;
            int y;

            do
            {
                x = _rnd.Next(0, Map.GetLength(0));
                y = _rnd.Next(0, Map.GetLength(1));

            } while (Map[x, y] == null || !Map[x, y].Walkable());

            return new Point(x, y);
        }

        /// <summary>
        /// Is the provided coordinate walkable
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public bool IsWalkable(int pX, int pY)
        {
            if (pX < 0 || pX >= Map.GetLength(0) || pY < 0 || pY >= Map.GetLength(1))
                return false;

            return Map[pX, pY] != null && Map[pX, pY].Walkable();
        }

        /// <summary>
        /// Is the specified point visible, i.e. legal and not solid
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public bool IsVisible(int pX, int pY)
        {
            if (pX < 0 || pX >= Map.GetLength(0) || pY < 0 || pY >= Map.GetLength(1))
                return false;

            return Map[pX, pY] != null && Map[pX, pY].Solid == 0;
        }

        /// <summary>
        /// Can the player be seen by the specified creature
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        private bool CanSeePlayer(Monster pMonster, Player pPlayer)
        {
            if (Math.Sqrt(Math.Pow(pPlayer.X - pMonster.X, 2) + Math.Pow(pPlayer.Y - pMonster.Y, 2)) > pMonster.VisionRange)
                return false;

            return true;
        }
    }
}
