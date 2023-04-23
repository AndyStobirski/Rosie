using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Items;
using Rosie.Code.Misc;
using Rosie.Entities;
using System;
using System.Linq;


namespace Rosie
{
    /// <summary>
    /// Handle the game commands, moved to a partial to keep the class easy to navigate
    /// </summary>
    public partial class RosieGame
    {
        /// <summary>
        /// Excecute the game command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void GameCommand(CommandType command, int[] data)
        {

            Point dir = Point.Zero;

            switch (command)
            {
                case CommandType.Move:

                    dir = GetVectorFromDirection(data.First());
                    if (MapUtils.IsWalkable(player.X + dir.X, player.Y + dir.Y))
                    {
                        player.Move(dir.X, dir.Y);
                    }
                    else if (MapUtils.ContainsMonster(player.X + dir.X, player.Y + dir.Y))
                    {
                        var m = Map[player.X + dir.X, player.Y + dir.Y].Inhabitant;
                        if (player.CanAttack(m))
                        {
                            player.Attack(m);
                        }
                    }
                    break;

                case CommandType.Take:

                    Item tItem = null;
                    if (MapUtils.ItemTake(player.X, player.Y, out tItem))
                    {
                        RosieGame.AddMessage(MessageStrings.Take_True, tItem.Name);
                        player.TakeItem(tItem);
                    }
                    else
                    {
                        RosieGame.AddMessage(MessageStrings.Take_False);
                    }
                    break;

                case CommandType.Drop:

                    if (Map[player.X, player.Y] is not Tile)
                    {
                        AddMessage(MessageStrings.Drop_No, null);
                        return;
                    }

                    int dropItem = (int)data.Last();

                    var item = player.Inventory[dropItem - (int)keys.keyA];
                    player.DropItem(item);

                    MapUtils.PlayerDropItem(item, player.X, player.Y);
                    AddMessage(MessageStrings.Drop_DropItem, item.Name);

                    break;

                case CommandType.Equip:

                    int equipIndex = (int)data.Last();

                    var equipItem = player.Inventory[equipIndex - (int)keys.keyA];

                    if (equipItem is Armour || equipItem is Weapon)
                    {
                        if (equipItem is Armour)
                        {
                            player.EquipArmour(equipItem as Armour);
                        }
                        else
                        {
                            player.EquipWeapon(equipItem as Weapon);
                        }

                        AddMessage(MessageStrings.Equip_Equip, equipItem.Name);
                    }
                    break;

                case CommandType.Open:
                    dir = GetVectorFromDirection(data.Last());
                    MapUtils.DoorStateChange(player.X + dir.X, player.Y + dir.Y, true);
                    break;


                case CommandType.Close:
                    dir = GetVectorFromDirection(data.Last());
                    MapUtils.DoorStateChange(player.X + dir.X, player.Y + dir.Y, false);
                    break;


                case CommandType.StairsMove:
                    UseStairs(player.X, player.Y);
                    break;

                case CommandType.MiniMap:
                    ViewMode = GameViewMode.MiniMap;
                    break;

                case CommandType.Look:
                    Look(data[1], data[2]);
                    break;

                default:
                    throw new NotImplementedException("GameCommand: " + command.ToString());

            }

            CalculateFieldOfVision();
            GameState = GameStates.EnemyTurn;
        }


        /// <summary>
        /// Move a player between levels
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        private void UseStairs(int pX, int pY)
        {
            var stairs = currentLevel.Map[pX, pY] as Staircase;

            if (stairs != null)
            {
                if (stairs.IsUp)
                {

                    currentLevel.Map[player.X, player.Y].Inhabitant = null;
                    RosieGame.AddMessage(MessageStrings.Stairs_Up);
                    GetLevel(-1);

                }
                else
                {
                    currentLevel.Map[player.X, player.Y].Inhabitant = null;
                    RosieGame.AddMessage(MessageStrings.Stairs_Down);
                    GetLevel(1);
                }
            }
            else
            {
                RosieGame.AddMessage(MessageStrings.Stairs_None);
            }
        }

        private readonly keys[] DirectionKeys = new keys[]
    {
                keys.keypad1, keys.keypad2, keys.keypad3
                , keys.keypad4, keys.keypad5, keys.keypad6
                , keys.keypad7, keys.keypad8, keys.keypad9
    };


        private Point GetVectorFromDirection(int pKey)
        {
            var idx = DirectionKeys.ToList().FindIndex(k => (int)k == pKey);
            return Library.Directions[idx];
        }

        private void Look(int pX, int pY)
        {
            var thing = Map[pX, pY];

            RosieGame.AddMessage(thing.Description());

            if (thing is Tile)
            {
                var t = thing as Tile;

                if (t.Inhabitant != null)
                {
                    RosieGame.AddMessage(MessageStrings.See_YouSee + (t.Inhabitant as NPC).Description());
                }

                if (t.Items.Any())
                {
                    RosieGame.AddMessage(MessageStrings.See_YouSee);

                    foreach (var item in t.Items)
                    {
                        RosieGame.AddMessage(item.Description());
                    }
                }
            }


        }


    }
}
