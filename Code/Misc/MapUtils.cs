using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Interfaces;
using Rosie.Code.Items;
using Rosie.Code.Map;
using Rosie.Entities;
using System;
using System.Linq;

namespace Rosie.Code.Misc
{
    /// <summary>
    /// A set of helpful utilities that access the static items in the game containing
    /// relating to the current level
    /// </summary>
    public static class MapUtils
    {
        private static Random rnd = new Random();
        private static Level currentLevel => RosieGame.currentLevel;
        private static Tile[,] Map => RosieGame.currentLevel.Map;

        /// <summary>
        /// Is the provided coordinate walkable
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public static bool IsWalkable(int pX, int pY)
        {
            if (pX < 0 || pX >= Map.GetLength(0) || pY < 0 || pY >= Map.GetLength(1))
                return false;

            return Map[pX, pY] != null && Map[pX, pY].Passable();
        }

        /// <summary>
        /// Is the specified cell existance?
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public static bool IsCellValid(int pX, int pY)
        {
            if (pX < 0 || pX >= Map.GetLength(0) || pY < 0 || pY >= Map.GetLength(1))
                return false;
            if (Map[pX, pY] == null) return false;

            return true;
        }

        public static bool ContainsMonster(int pX, int pY)
        {
            if (pX < 0 || pX >= Map.GetLength(0) || pY < 0 || pY >= Map.GetLength(1))
                return false;

            return Map[pX, pY] != null && Map[pX, pY].Inhabitant is NPC;
        }

        /// <summary>
        /// Return a random walkable cell from the map
        /// </summary>
        /// <returns></returns>
        public static Point RandomWalkableCell()
        {
            int x;
            int y;

            do
            {
                x = rnd.Next(0, Map.GetLength(0));
                y = rnd.Next(0, Map.GetLength(1));

            } while (Map[x, y] == null || !Map[x, y].Passable());

            return new Point(x, y);
        }



        static Point[] Directions =
            {
                  new Point(-1, 1),  new Point(0, 1),   new Point(1, 1)
                , new Point(-1, 0),                     new Point(1, 0)
                , new Point(-1, -1), new Point(0, -1),  new Point(1,-1)
            };

        /// <summary>
        /// Return a list of 9 points points which surrounding the provided point
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public static Point[] GetSurroundingPoints(int pX, int pY)
        {
            return Directions.Select(p => new Point(p.X + pX, p.Y + pY)).ToArray();
        }

        /// <summary>
        /// Attempt to pick up an item on the specified tile
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pItem"></param>
        /// <returns></returns>
        public static bool ItemTake(int pX, int pY, out Item pItem)
        {
            pItem = null;

            if (Map[pX, pY].Items == null || !Map[pX, pY].Items.Any())
                return false;

            pItem = Map[pX, pY].Items.Last();
            Map[pX, pY].Items.Remove(pItem);
            return true;
        }

        /// <summary>
        /// Drop the item on the specified tile
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pItem"></param>
        public static void ItemDrop(int pX, int pY, Item pItem)
        {
            Map[pX, pY].Items.Add(pItem);
        }

        /// <summary>
        /// Return a random empty point in a random room
        /// </summary>
        /// <returns></returns>
        public static Point GetRandomRoomPoint()
        {
            var p = new Point();

            do
            {
                var room = currentLevel.Rooms[rnd.Next(0, currentLevel.Rooms.Count)];
                p = new Point(
                    rnd.Next(room.Left, room.Right)
                    , rnd.Next(room.Top, room.Bottom)
                    );

            } while (!(Map[p.X, p.Y] is Floor) && !Map[p.X, p.Y].Passable());

            return p;
        }


        public static void MoveActor(int pOldX, int pOldY, int pNewX, int pNewY, Actor pActor)
        {

        }

        /// <summary>
        /// Place the provided item on the current map
        /// </summary>
        /// <param name="pItem"></param>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        public static void PlayerDropItem(Item pItem, int pX, int pY)
        {
            pItem.X = pX;
            pItem.Y = pY;
            Map[pX, pY].Items.Add(pItem);
        }


        /// <summary>
        /// Calculate the distance between the points using the Pythagorean theorem
        /// </summary>
        /// <returns></returns>
        public static int CellDistance(int pX1, int pY1, int pX2, int pY2)
        {
            int dx = pX2 - pX1;
            int dy = pY2 - pY1;
            int distance = (int)Math.Sqrt(dx * dx + dy * dy);

            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Change the state of the door on the specified cell
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pSetToOpen"></param>
        public static void DoorStaeChange(int pX, int pY, bool pSetToOpen)
        {

            if (!IsCellValid(pX, pY) || Map[pX, pY] is not iOpenable)
            {
                if (pSetToOpen)
                {
                    RosieGame.AddMessage(MessageStrings.Open_No);
                }
                else
                {
                    RosieGame.AddMessage(MessageStrings.Close_No);
                }
            }

            var door = Map[pX, pY] as iOpenable;

            if (pSetToOpen)
            {
                //opening it
                if (door.IsOpen)
                {
                    RosieGame.AddMessage(MessageStrings.Open_No);
                }
                else
                {
                    door.IsOpen = true;
                }
            }
            else
            {
                //cloosing it
                if (!door.IsOpen)
                {
                    RosieGame.AddMessage(MessageStrings.Close_No);
                }
                else
                {
                    door.IsOpen = false;
                }
            }
        }
    }
}
