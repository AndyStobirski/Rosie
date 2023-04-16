using Microsoft.Xna.Framework;
using Rosie.Code;
using Rosie.Code.Environment;
using Rosie.Code.Items;
using Rosie.Code.Misc;
using Rosie.Enums;
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
                        RosieGame.AddMessage(Code.MessageStrings.Take_True, tItem.Name);
                        player.TakeItem(tItem);
                    }
                    else
                    {
                        RosieGame.AddMessage(Code.MessageStrings.Take_False);
                    }
                    break;

                case CommandType.Drop:

                    //
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

    }
}
