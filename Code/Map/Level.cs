using Microsoft.Xna.Framework;
using Rosie.Code.Actors;
using Rosie.Code.Environment;
using Rosie.Code.Items;
using Rosie.Code.Items.Weapons;
using Rosie.Code.Misc;
using Rosie.Code.sensedata;
using Rosie.Code.TextGenerators;
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

        /// <summary>
        /// Player
        /// </summary>
        public Player player { get; set; }

        /// <summary>
        /// The Level map
        /// </summary>
        public Tile[,] Map { get; set; }

        /// <summary>
        /// Rectangles represent map rooms
        /// </summary>
        public List<Rectangle> Rooms { get; set; }

        /// <summary>
        /// A point in the centre of a room, used for NPC navigation
        /// </summary>
        public List<WayPoint> WayPoints { get; set; }

        /// <summary>
        /// Player / NPC generated sensedata stored here
        /// </summary>
        public List<SenseDatum> SenseData { get; set; }

        public Point StairCase_Up { get; set; }
        public Point StairCase_Down { get; set; }

        public Guid Guid { get; set; }

        public void Monster_ActorMoved(object sender, Actor.ActorCompeletedTurnEventArgs e)
        {
            Map[e.Before.X, e.Before.Y].Inhabitant = null;
            Map[e.After.X, e.After.Y].Inhabitant = e.Inhabitant;
        }

        public Level()
        {
            Guid = Guid.NewGuid();
            SenseData = new List<SenseDatum>();
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
        public void InitLevel(Player pPlayer, int pMaxMonsters)
        {

            player = pPlayer;

            if (!_Scheduler.ContainsActor(pPlayer))
            {
                _Scheduler.AddActor(pPlayer);
            }

            NPC m;

            Point location;
            WayPoint wp;

            //
            //  Set NPCs
            //
            for (int ctr = 0; ctr < pMaxMonsters; ctr++)
            {
                MapUtils.GetRandomRoomPoint(out location, out wp);

                m = new NPC(location, new ScriptBasic1())
                {
                    Gfx = (int)GFXValues.MONSTER_ORC
                    ,
                    Speed = 10
                    ,
                    VisionRange = 5
                    ,
                    HitPointsMax = 5
                    ,
                    HitPointsCurrent = 5
                    ,
                    Class = "Orc"
                    ,
                    Name = GibberishGenerator.GenerateName()
                };


                m.Name = GibberishGenerator.GenerateName();
                m.script.SetTargetWayPoint(wp);
                m.WeaponPrimary = new Spear();
                m.ActorCompletedTurn += Monster_ActorMoved;
                m.Died += Monster_Died;

                Map[m.X, m.Y].Inhabitant = m;
                Monsters.Add(m);
                _Scheduler.AddActor(m);
            }


            /*
            for (int ctr = 0; ctr < pMaxMonsters; ctr++)
            {
                MapUtils.GetRandomRoomPoint(out location, out wp);

                m = new NPC(location, new ScriptZombie())
                {
                    Gfx = (int)GFXValues.MONSTER_SKELETON
                    ,
                    Speed = 10
                    ,
                    VisionRange = 5
                    ,
                    HitPointsMax = 5
                    ,
                    HitPointsCurrent = 5
                    ,
                    Class = "Skeleton"
                    ,
                    Name = GibberishGenerator.GenerateName(true)
                };


                m.script.SetTargetWayPoint(wp);
                m.WeaponPrimary = new Dagger();
                m.ActorCompletedTurn += Monster_ActorMoved;
                m.Died += Monster_Died;

                Map[m.X, m.Y].Inhabitant = m;
                Monsters.Add(m);
                _Scheduler.AddActor(m);
            }
            */



            //
            //  Randomly add treasure
            //
            for (int ctr = 0; ctr < 10; ctr++)
            {
                var p = GetRandomEmptyRoomPoint();
                AddItem(new GoldCoins(RandomWithSeed.Next(1, 100)) { X = p.X, Y = p.Y });
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
            RosieGame.AddMessage(MessageStrings.Monster_Die, e.monster.ID);
            player.ExperiencePoints += e.monster.ExperienceValue;
            Monsters.Remove(e.monster);
            _Scheduler.RemoveActor(e.monster);
            Map[e.monster.X, e.monster.Y].Inhabitant = null;

        }

        public Point GetRandomEmptyRoomPoint()
        {
            Point pLocation = new Point();

            do
            {
                var rommIdx = RandomWithSeed.Next(Rooms.Count);

                var room = Rooms[rommIdx];
                pLocation = new Point(
                    RandomWithSeed.Next(room.Left, room.Right)
                    , RandomWithSeed.Next(room.Top, room.Bottom)
                    );

                if (Map[pLocation.X, pLocation.Y] is Floor)
                {

                    var floor = Map[pLocation.X, pLocation.Y] as Floor;

                    if (floor.Passable() && floor.Inhabitant == null && floor.Items.Count == 0)
                    {
                        break;
                    }
                }

            } while (true);

            return pLocation;

        }


    }
}
