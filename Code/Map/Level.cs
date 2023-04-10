using Microsoft.Xna.Framework;
using Rosie.Code.Actors;
using Rosie.Code.Environment;
using Rosie.Code.Items;
using Rosie.Code.Items.Weapons;
using Rosie.Code.Misc;
using Rosie.Entities;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rosie.Code.Map
{
    public class Level
    {
        Scheduler _Scheduler = new Scheduler();


        public List<Item> Items = new List<Item>();

        /// <summary>
        /// Monsters that live on the current level
        /// </summary>
        public List<NPC> Monsters = new List<NPC>();

        public Player player { get; set; }

        public Tile[,] Map { get; set; }

        public List<Rectangle> Rooms { get; set; }

        public List<WayPoint> WayPoints { get; set; }

        protected Random _rnd = new Random();



        public void Monster_ActorMoved(object sender, Actor.ActorCompeletedTurnEventArgs e)
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
        /// A list of actors whose current turn it is to move
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Actor> ActorsToMove()
        {
            return _Scheduler.Tick();
        }



        /// <summary>
        /// Set up monsters on the 
        /// </summary>
        public void InitActors(Player pPlayer, int pMaxMonsters)
        {

            player = pPlayer;
            _Scheduler.AddActor(pPlayer);

            NPC m;

            Point location;
            WayPoint wp;


            for (int ctr = 0; ctr < pMaxMonsters; ctr++)
            {
                MapUtils.GetRandomRoomPoint(out location, out wp);

                m = new NPC(location)
                {
                    Gfx = (int)GFXValues.MONSTER_SKELETON
                    ,
                    Speed = 10
                    ,
                    VisionRange = 3
                    ,
                    HitPointsMax = 5,

                    HitPointsCurrent = 5
                };

                m.script = new ScriptZombie(m);
                m.script.SetTargetWayPoint(wp);
                m.WeaponPrimary = new Spear();
                m.ActorCompletedTurn += Monster_ActorMoved;
                m.Died += Monster_Died;


                //Console.WriteLine("Monster {0},{1}", m.RoamTarget.X, m.RoamTarget.Y);

                Map[m.X, m.Y].Inhabitant = m;
                Monsters.Add(m);
                _Scheduler.AddActor(m);
            }
        }

        /// <summary>
        /// Add the provided item to the level collection and register it with the map
        /// </summary>
        /// <param name="pItem"></param>
        public void AddItem(Item pItem)
        {
            Items.Add(pItem);
            Map[pItem.X, pItem.Y].Items.Add(pItem);
        }

        /// <summary>
        /// Monster dead
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Monster_Died(object sender, NPC.MonsterDiedEventArgs e)
        {
            RosieGame.AddMessage(MessageStrings.Monster_Die, e.monster.Name);
            player.ExperiencePoints += e.monster.ExperienceValue;
            Monsters.Remove(e.monster);
            _Scheduler.RemoveActor(e.monster);
            Map[e.monster.X, e.monster.Y].Inhabitant = null;

        }


    }
}
