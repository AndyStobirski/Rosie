using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Map;
using Rosie.Code.Misc;
using Rosie.Entities;
using Rosie.Enums;
using Rosie.Map;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Rosie
{
    /// <summary>
    /// The main game container
    /// </summary>
    public partial class RosieGame
    {
        private Random rnd = new Random();

        private MapGenerator mapGenerator = new MapGenerator();

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
        public Size MapSize => Map == null ? null : new Size(Map.GetLength(0), Map.GetLength(1));

        /// <summary>
        /// Defines the rectangle which contains the camera view
        /// </summary>
        public Rectangle CameraBorder => new Rectangle(GameCameraOffset.X, GameCameraOffset.Y, GameCameraSize.X * TileSize.Width, GameCameraSize.Y * TileSize.Height);

        #endregion

        /// <summary>
        /// Game turns
        /// </summary>
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

        public Tile[,] Map => currentLevel?.Map;


        List<Level> levels = new List<Level>();

        /// <summary>
        /// Why static? So this can be easily accessed by the class Monster
        /// They're nested quite deeply, and rather than pass variables through
        /// a series of constructors
        /// </summary>
        static public Level currentLevel;

        /// <summary>
        /// Current level index
        /// </summary>
        public int currentLevelIndex => levels.IndexOf(currentLevel);

        /// <summary>
        /// Why static? So this can be easily accessed by the class Monster
        /// They're nested quite deeply, and rather than pass variables through
        /// a series of constructors
        /// </summary>
        static public Player player;



        public static void AddMessage(string pMessageBody, params object[] pArgs)
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


        public Level CreateNewLevel()
        {

            var level = mapGenerator.Build(100, 100);
            return level;
        }

        /// <summary>
        /// Get the required level
        /// </summary>
        /// <param name="pLevelIndex"></param>
        public void GetLevel(int pLevelIndex)
        {

            if (levels.Count == 0)
            {
                currentLevel = CreateNewLevel();
                levels.Add(currentLevel);
                currentLevel.InitLevel(player, 0);
                player.X = currentLevel.StairCase_Up.X;
                player.Y = currentLevel.StairCase_Up.Y;

            }
            else if (pLevelIndex == -1)
            {

                if ((currentLevelIndex + pLevelIndex) >= 0)
                {

                    RosieGame.AddMessage(Code.MessageStrings.Stairs_Up);

                    currentLevel = levels[currentLevelIndex + pLevelIndex];

                    //moving up
                    player.X = currentLevel.StairCase_Down.X;
                    player.Y = currentLevel.StairCase_Down.Y;
                }
                else
                {
                    RosieGame.AddMessage(Code.MessageStrings.Stairs_CantLeave);
                }
            }
            else if (pLevelIndex == 1)
            {

                if ((currentLevelIndex + pLevelIndex) > levels.Count() - 1)
                {

                    levels.Add(CreateNewLevel());
                    currentLevel = levels.Last();
                    currentLevel.InitLevel(player, 0);
                }
                else
                {
                    currentLevel = levels[currentLevelIndex + pLevelIndex];
                }

                //moving down
                player.X = currentLevel.StairCase_Up.X;
                player.Y = currentLevel.StairCase_Up.Y;
            }


            PlacePlayerInCurrentLevel(player.X, player.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        public void PlacePlayerInCurrentLevel(int pX, int pY)
        {
            player.X = pX;
            player.Y = pY;
            currentLevel.Map[pX, pY].Inhabitant = player;

            _fov = new FOVRecurse(player, Map);

            CalculateGameCameraDefinition();
            MapUtils.MakeMapVisible();

        }


        /// <summary>
        /// Constructor
        /// </summary>
        public RosieGame()
        {
            // TODO Need to refine the code that follows

            mapGenerator = new MapGenerator();
            //{
            //    Room_Min = new Size(4, 4),
            //    Room_Max = new Size(10, 10),
            //    MaxRooms = 2,
            //    MapSize = new Size(100, 100),
            //    RoomDistance = 3,
            //    Corridor_Max = 10,
            //    Corridor_MaxTurns = 5
            //};


            //
            //  Player
            //
            player = new Player();
            player.ActorCompletedTurn += Player_ActorMoved;

            GameState = Enums.GameStates.PlayerTurn;

            GetLevel(0);



            //
            //  Write waypoint data for testing
            //
            using (StreamWriter sw = new StreamWriter(@"c:\temp\waypoints.txt"))
            {
                sw.WriteLine("digraph g");
                sw.WriteLine("{");
                sw.WriteLine("node [shape=box]");
                sw.WriteLine("graph [splines=ortho]");

                foreach (var wp in currentLevel.WayPoints)
                {
                    foreach (var cp in wp.ConnectedPoints)
                    {
                        sw.WriteLine(string.Format("\t{0} -> {1}", currentLevel.WayPoints.IndexOf(wp), currentLevel.WayPoints.IndexOf(cp)));
                    }
                }

                sw.WriteLine("}");
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
        private readonly keys[] DirectionKeys = new keys[]
            {
                keys.keypad1, keys.keypad2, keys.keypad3
                , keys.keypad4, keys.keypad5, keys.keypad6
                , keys.keypad7, keys.keypad8, keys.keypad9
            };


        readonly Point[] Directions =
            {
                  new Point(-1, 1),  new Point(0, 1),   new Point(1, 1)
                , new Point(-1, 0),  new Point(0, 0),   new Point(1, 0)
                , new Point(-1, -1), new Point(0, -1),  new Point(1,-1)
            };


        private Point GetVectorFromDirection(int pKey)
        {
            var idx = DirectionKeys.ToList().FindIndex(k => (int)k == pKey);
            return Directions[idx];
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

        public string PlayerProperties()
        {
            return String.Join("\r\n",
                new string[]
                {
                        player.Name
                        , "HP: " + string.Format("{0}/{1}", player.HitPointsCurrent, player.HitPointsMax)
                        , "XP: " + player.ExperiencePoints
                        , "PT: " + string.Format("{0},{1}", player.X, player.Y)
                        , "GD: " + player.Gold.ToString("X")
                        , "AEQ: " + (player.ArmourEquiped == null ? " - " : player.ArmourEquiped.Name)
                        , "WEQ: " + (player.WeaponPrimary == null ? " - " : player.WeaponPrimary.Name)
                        , "LVL : " + (currentLevelIndex + 1).ToString("X")
                    }
                );
        }

        #endregion

    }
}
