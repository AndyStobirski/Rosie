using Microsoft.Xna.Framework;
using Rosie.Code;
using Rosie.Code.Environment;
using Rosie.Code.Items;
using Rosie.Code.Map;
using Rosie.Code.Misc;
using Rosie.Entities;
using Rosie.Enums;
using Rosie.Map;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rosie
{
    /// <summary>
    /// The main game container
    /// </summary>
    public class RosieGame
    {
        private Random rnd = new Random();

        #region Keymodifiers

        // The state of the keyboard modifiers as set in the MonoGame parent class
        // we need these as keypress reporting isn't great, e.g. A reports as ascii 
        // code 65 regardless of capslock or shift state, doesn't reflect the code 
        // being 65 or 97

        public bool CapsLock { get; set; }
        public bool NumLock { get; set; }
        public bool ShiftDown { get; set; }

        #endregion

        /// <summary>
        /// Tile selected by mouse click
        /// </summary>
        public static Point SelectedTile { get; set; }

        /// <summary>
        /// Game messages are stored here. Why static? Easy for other things to access this.
        /// </summary>
        public static List<string> Messages { get => messages; set => messages = value; }
        private static List<string> messages = new List<string>();

        #region Map Draw Properties
        /// <summary>
        /// The offset to start drawing the game view based on the top left
        /// hand of the ganme 
        /// </summary>
        public Point GameCameraOffset { get; set; } = new Point(320, 32);

        /// <summary>
        /// The size of the tiles to draw
        /// </summary>
        public Size TileSize { get; } = new Size(32, 32);

        /// <summary>
        /// The rectangle which defines the GameView on the map, recalculate every time the player moves
        /// </summary>
        private Rectangle _GameCameraDefinition;
        public Rectangle GameCameraDefinition => _GameCameraDefinition;

        /// <summary>
        /// The size of the gameview, which a portion of the map centered
        /// arround the player
        /// </summary>
        public Point GameCameraSize { get; set; } = new Point(25, 25);


        /// <summary>
        /// Size of the GameMap in tiles
        /// </summary>
        public Size MapSize => new Size(Map.GetLength(0), Map.GetLength(1));

        /// <summary>
        /// Defines the rectangle which contains the camera view
        /// </summary>
        public Rectangle CameraBorder => new Rectangle(GameCameraOffset.X, GameCameraOffset.Y, GameCameraSize.X * TileSize.Width, GameCameraSize.Y * TileSize.Height);

        #endregion

        public int TurnCounter { get; set; }


        /// <summary>
        /// Player vision is calculated here
        /// </summary>
        public FOVRecurse _fov;

        /// <summary>
        /// Calculate the field of vision, the visible cells that the player can see
        /// </summary>
        private void CalculateFieldOfVision()
        {
            _fov.GetVisibleCells(_GameCameraDefinition);
        }


        #region The Game mode determines what is displayed

        public static GameViewMode ViewMode { get; set; }

        #endregion

        public Tile[,] Map => currentLevel.Map;


        List<Level> levels = new List<Level>();

        /// <summary>
        /// Why static? So this can be easily accessed by the class Monster
        /// They're nested quite deeply, and rather than pass variables through
        /// a series of constructors
        /// </summary>
        static public Level currentLevel;

        /// <summary>
        /// Why static? So this can be easily accessed by the class Monster
        /// They're nested quite deeply, and rather than pass variables through
        /// a series of constructors
        /// </summary>
        static public Player player;



        public static void AddMessage(string pMessageBody, params string[] pArgs)
        {
            Messages.Insert(0, string.Format(pMessageBody, pArgs));
        }

        /// <summary>
        /// Calculate the GameView, a rectangle which defines the drawn part of the map
        /// based on the location of the player
        /// </summary>
        public void CalculateGameCameraDefinition()
        {

            // GameView
            _GameCameraDefinition.X = player.X - GameCameraSize.X / 2;
            _GameCameraDefinition.Y = player.Y - GameCameraSize.Y / 2;

            if (_GameCameraDefinition.X < 0)
                _GameCameraDefinition.X = 0;
            else if (_GameCameraDefinition.X + GameCameraSize.X > MapSize.Width)
                _GameCameraDefinition.X -= (_GameCameraDefinition.X + GameCameraSize.X - MapSize.Width);

            if (_GameCameraDefinition.Y < 0)
                _GameCameraDefinition.Y = 0;
            else if (_GameCameraDefinition.Y + GameCameraSize.Y > MapSize.Height)
                _GameCameraDefinition.Y -= (_GameCameraDefinition.Y + GameCameraSize.Y - MapSize.Height);

            _GameCameraDefinition.Width = GameCameraSize.X;
            if (_GameCameraDefinition.Right > MapSize.Width)
                _GameCameraDefinition.Width -= (_GameCameraDefinition.Right - MapSize.Width);

            _GameCameraDefinition.Height = GameCameraSize.Y;
            if (_GameCameraDefinition.Bottom > MapSize.Height)
                _GameCameraDefinition.Height -= (_GameCameraDefinition.Bottom - MapSize.Height);


            //Calculate visible cells
            _fov.GetVisibleCells(_GameCameraDefinition);
        }


        public GameStates GameState { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public RosieGame()
        {
            // TODO Need to refine the code that follows

            var cg = new MapGenerator()
            {
                Room_Min = new Size(4, 4),
                Room_Max = new Size(10, 10),
                MaxRooms = 3,
                MapSize = new Size(50, 50),
                RoomDistance = 3,
                Corridor_Max = 10,
                Corridor_MaxTurns = 5
            };
            //var cg = new CorridorCaveGenerator();

            levels.Add(cg.Build());

            //  Init the player
            var pt = cg.GetStartLocation();
            player = new Player
            {
                X = pt.X,
                Y = pt.Y
            };

            player.ActorCompletedTurn += Player_ActorMoved;

            currentLevel = levels.First();
            currentLevel.InitActors(player, 3);

            GameState = Enums.GameStates.PlayerTurn;

            currentLevel.Map[player.X, player.Y].Inhabitant = player;

            _fov = new FOVRecurse(player, Map);

            CalculateGameCameraDefinition();


            //  Randomly add treasure
            for (int ctr = 0; ctr < 10; ctr++)
            {
                var p = MapUtils.GetRandomRoomPoint();
                currentLevel.AddItem(new GoldCoins(rnd.Next(1, 100)) { X = p.X, Y = p.Y });
            }



        }

        /// <summary>
        /// Get a list of actors who's turn it to move from the current level
        /// </summary>
        public void Tick()
        {

            IEnumerable<Actor> _ActorsToMove = currentLevel.ActorsToMove().ToList();

            if (_ActorsToMove == null)
                return;

            if (_ActorsToMove.Contains(player))
            {
                TurnCounter++;
                GameState = GameStates.PlayerTurn;
            }

            foreach (Actor a in _ActorsToMove.Where(ac => ac is NPC))
            {
                ((NPC)a).Act();
            }

        }

        /// <summary>
        /// Raise an event that activity has occured within the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Player_ActorMoved(object sender, Actor.ActorCompeletedTurnEventArgs e)
        {

            Map[e.Before.X, e.Before.Y].Inhabitant = null;
            Map[e.After.X, e.After.Y].Inhabitant = e.Inhabitant;

            GameState = GameStates.EnemyTurn;
            CalculateGameCameraDefinition();
            CalculateFieldOfVision();

        }


        #region mouse / key handling

        // Numpad 1 to 9, excluding 5

        private readonly keys[] DirectionKeys = new keys[]
{
            keys.keypad1, keys.keypad2, keys.keypad3
            , keys.keypad4,  keys.keypad6
            , keys.keypad7, keys.keypad8, keys.keypad9
};



        readonly Point[] Directions =
            {
                  new Point(-1, 1),  new Point(0, 1),   new Point(1, 1)
                , new Point(-1, 0),                     new Point(1, 0)
                , new Point(-1, -1), new Point(0, -1),  new Point(1,-1)
            };


        private Point GetVectorFromDirection(int pKey)
        {
            var idx = DirectionKeys.ToList().FindIndex(k => (int)k == pKey);
            return Directions[idx];
        }


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
                    MapUtils.DoorStaeChange(player.X + dir.X, player.Y + dir.Y, true);
                    break;


                case CommandType.Close:
                    dir = GetVectorFromDirection(data.Last());
                    MapUtils.DoorStaeChange(player.X + dir.X, player.Y + dir.Y, false);
                    break;


                default:
                    throw new NotImplementedException("GameCommand: " + command.ToString());

            }

            CalculateFieldOfVision();
            GameState = GameStates.EnemyTurn;
        }

        /// <summary>
        /// A click has occured on the game window
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        public void MouseClick(int pX, int pY)
        {
            Debug.WriteLine(string.Format("Mouse click X:{0}, Y:{1}", pX, pY));
        }

        #endregion

    }
}
