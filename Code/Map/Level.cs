using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Items;
using Rosie.Code.Misc;
using Rosie.Code.sensedata;
using Rosie.Code.TextGenerators;
using Rosie.Entities;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                m = EntityData.RandomNPC();

                MapUtils.GetRandomRoomPoint(out location, out wp);


                m.PlaceNPC(location.X, location.Y, wp);


                m.Name = GibberishGenerator.GenerateName();
                m.script.SetTargetWayPoint(wp);
                m.WeaponPrimary = EntityData.RandomWeapon();

                m.ActorActivityOccured += M_ActorActivityOccured;

                Map[m.X, m.Y].Inhabitant = m;
                Monsters.Add(m);
                _Scheduler.AddActor(m);
            }



            //
            //  Randomly add treasure
            //
            for (int ctr = 0; ctr < 10; ctr++)
            {
                var p = GetRandomEmptyRoomPoint();
                AddItem(new GoldCoins(RandomWithSeed.Next(1, 100)) { X = p.X, Y = p.Y });
            }

            //
            //  Randomly add items
            //
            foreach (var w in EntityData.Weapons)
            {
                var w1 = EntityData.RandomWeapon();
                var p = GetRandomEmptyRoomPoint();
                w1.SetLocation(p.X, p.Y);
                AddItem(w1);
            }

            foreach (var a in EntityData.Armours)
            {
                var b = EntityData.RandomArmour();
                var p = GetRandomEmptyRoomPoint();
                b.SetLocation(p.X, p.Y);
                AddItem(b);
            }

        }

        /// <summary>
        /// When an actor does stuff it's reported here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M_ActorActivityOccured(object sender, Actor.ActorActivity e)
        {
            var m = sender as NPC;

            switch (e.Activity)
            {
                case ActorActivityType.Damaged:
                    Game1.AddTextEffect(e.Activity, m.X, m.Y, e.Data.First().ToString(), 0, -1, Color.Red);
                    break;

                case ActorActivityType.Died:
                    var monster = sender as NPC;
                    RosieGame.AddMessage(MessageStrings.Monster_Die, monster.ID);
                    player.ExperiencePoints += monster.ExperienceValue;
                    Monsters.Remove(monster);
                    _Scheduler.RemoveActor(monster);
                    Map[monster.X, monster.Y].Inhabitant = null;
                    break;

                case ActorActivityType.Moved:

                    //  e.Data - before X, before Y, current X, current Y
                    var coords = e.Data.Select(n => (int)n).ToArray();

                    Map[coords[0], coords[1]].Inhabitant = null;
                    Map[coords[2], coords[3]].Inhabitant = sender as NPC;
                    break;

                default:
                    throw new Exception("Case not handled");
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
