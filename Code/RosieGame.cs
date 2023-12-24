using Microsoft.Xna.Framework;
using Rosie.Code;
using Rosie.Code.Environment;
using Rosie.Code.Map;
using Rosie.Code.Misc;
using Rosie.Code.sensedata;
using Rosie.Entities;
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
        // we need these as keypress reporting isn't great, e.g. A report of ascii 
        // code 65 regardless of capslock or shift state, doesn't reflect the code 
        // being 65 or 97

        public bool CapsLock { get; set; }
        public bool NumLock { get; set; }
        public bool ShiftDown { get; set; }

        #endregion

        /// <summary>
        /// Tile the mouse is over in the map
        public static Point MouseOverTile { get; set; }


        /// <summary>
        /// Game messages are stored here. Why static? Easy for other things to access this.
        /// </summary>
        public static List<string> Messages { get => messages; set => messages = value; }
        private static List<string> messages = new List<string>();


        /// <summary>
        /// Game turns
        /// </summary>
        public int TurnCounter { get; set; }

        /// <summary>
        /// Draw the odour club in the game, debug purposes only
        /// </summary>
        public bool DislayOdourCloud { get; set; } = true;

        /// <summary>
        /// Player vision is calculated here
        /// </summary>
        public RecursiveShadowcast _fov;


        /// <summary>
        /// Constructor for the entire game, it all starts here
        /// </summary>
        public RosieGame()
        {
            // TODO Need to refine the code that follows

            mapGenerator = new MapGenerator();
            mapGenerator.Corridor_Max = 5;
            mapGenerator.Corridor_Min = 3;
            mapGenerator.Corridor_MaxTurns = 1;
            mapGenerator.MaxRooms = 10;



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
            player.ActorActivityOccured += Player_ActorActivityOccured;

            GameState = GameStates.PlayerTurn;

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
        /// Calculate the field of vision, the visible cells that the player can see
        /// </summary>
        private void CalculateFieldOfVision()
        {

            //_fov.CalculateDistanceMap(7, player.X, player.Y, player.VisionRange + 5);
            //_fov.OutputMask();

            _fov.CastLight(Camera.GameCameraDefinition, player.X, player.Y, player.VisionRange);
        }


        /// <summary>
        /// The Game mode determines what is displayed on the main screen
        /// </summary>
        public static GameViewMode ViewMode { get; set; }

        /// <summary>
        /// If not null, this will be drawn by the game
        /// </summary>
        public string PopupWindow { get; set; }


        public Tile[,] Map => currentLevel.Map;


        List<Level> levels = new List<Level>();

        /// <summary>
        /// Why static? So this can be easily accessed by the class Monster
        /// They're nested quite deeply, and rather than pass variables through
        /// a series of constructors
        /// </summary>
        static public Level currentLevel;

        static private int _maxLevel;
        public int MaxLevel
        {
            get
            {

                if (currentLevelIndex > _maxLevel)
                    _maxLevel = currentLevelIndex;

                return _maxLevel;
            }
        }

        /// <summary>
        /// Current level index
        /// </summary>
        public int currentLevelIndex => levels.IndexOf(currentLevel);

        /// <summary>
        /// The player class!
        /// Why static? So this can be easily accessed by the class Monster
        /// They're nested quite deeply, and rather than pass variables through
        /// a series of constructors
        /// </summary>
        static public Player player;


        /// <summary>
        /// Add an entry to the message list
        /// </summary>
        /// <param name="pMessageBody"></param>
        /// <param name="pArgs"></param>
        public static void AddMessage(string pMessageBody, params object[] pArgs)
        {
            Messages.Insert(0, string.Format(pMessageBody, pArgs));
        }

        /// <summary>
        /// Determines what is being displayed
        /// </summary>
        public GameStates GameState { get; set; }

        /// <summary>
        /// Generate a new random level
        /// </summary>
        /// <returns></returns>
        public Level CreateNewLevel()
        {
            var level = mapGenerator.Build(50, 50);
            return level;
        }

        /// <summary>
        /// Set the current level to the one specified by the parameter
        /// </summary>
        /// <param name="pLevelIndex">Index of level required</param>
        public void GetLevel(int pLevelIndex)
        {

            if (levels.Count == 0)  //no levels present, create on
            {
                currentLevel = CreateNewLevel();
                levels.Add(currentLevel);
                currentLevel.InitLevel(player, 50);
                player.X = currentLevel.StairCase_Up.X;
                player.Y = currentLevel.StairCase_Up.Y;

            }
            else if (pLevelIndex == -1) // go up a flor
            {

                if ((currentLevelIndex + pLevelIndex) >= 0)
                {

                    RosieGame.AddMessage(MessageStrings.Stairs_Up);

                    currentLevel = levels[currentLevelIndex + pLevelIndex];

                    // Play the player on the new levels staircase
                    player.X = currentLevel.StairCase_Down.X;
                    player.Y = currentLevel.StairCase_Down.Y;
                }
                else
                {
                    RosieGame.AddMessage(MessageStrings.Stairs_CantLeave);
                }
            }
            else if (pLevelIndex == 1)  // go down a floor
            {

                if ((currentLevelIndex + pLevelIndex) > levels.Count() - 1)
                {

                    levels.Add(CreateNewLevel());
                    currentLevel = levels.Last();
                    currentLevel.InitLevel(player, 5);
                }
                else
                {
                    currentLevel = levels[currentLevelIndex + pLevelIndex];
                }

                // Play the player on the new levels staircase
                player.X = currentLevel.StairCase_Up.X;
                player.Y = currentLevel.StairCase_Up.Y;
            }


            PlacePlayerInCurrentLevel(player.X, player.Y);
        }

        /// <summary>
        /// The player has entered the current level
        /// </summary>
        /// <param name="pX">X Location</param>
        /// <param name="pY">Y Location</param>
        public void PlacePlayerInCurrentLevel(int pX, int pY)
        {
            player.X = pX;
            player.Y = pY;
            currentLevel.Map[pX, pY].Inhabitant = player;
            _fov = new RecursiveShadowcast(Map);
            MapUtils.MakeMapVisible();
            Camera.CalculateGameCameraDefinition();
            CalculateFieldOfVision();
        }



        /// <summary>
        /// This event is called from the player class, and is used to notify the game when the play does stuff
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void Player_ActorActivityOccured(object sender, Actor.ActorActivity e)
        {
            var p = sender as Player;

            switch (e.Activity)
            {
                case ActorActivityType.Damaged:
                    Game1.AddTextEffect(e.Activity, p.X, p.Y, e.Data.First().ToString(), 0, -1, Color.DarkOrange);
                    break;

                case ActorActivityType.Died:
                    throw new Exception("Plater died");

                case ActorActivityType.Moved:
                    var coords = e.Data.Select(n => (int)n).ToArray();

                    Map[coords[0], coords[1]].Inhabitant = null;
                    Map[coords[2], coords[3]].Inhabitant = sender as Player;

                    GameState = GameStates.EnemyTurn;
                    Camera.CalculateGameCameraDefinition();
                    CalculateFieldOfVision();
                    break;

                default:
                    throw new Exception("Case not handled");
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

                ScentPropogration();
            }

            foreach (Actor a in _ActorsToMove.Where(ac => ac is NPC))
            {
                ((NPC)a).Act();
            }
        }

        /// <summary>
        /// Calculate the propogation of player scent across the map
        /// </summary>
        private void ScentPropogration()
        {

            Scent s = currentLevel.SenseData.FirstOrDefault(s => s.X == player.X && s.Y == player.Y) as Scent;

            if (s == null)
            {
                //add it
                s = new Scent(player.BaseScent, 1, player.X, player.Y);
                Map[player.X, player.Y].SenseData.Add(s);
                currentLevel.SenseData.Add(s);
            }
            else
            {
                // top it up
                s.ScentValue = player.BaseScent;
            }

            // Examine the current contents of the sensedata 
            currentLevel.SenseData = currentLevel.SenseData.OrderByDescending(n => (n as Scent).ScentValue).ToList();
            for (int ctr = currentLevel.SenseData.Count - 1; ctr >= 0; ctr--)
            {
                var sc = currentLevel.SenseData[ctr] as Scent;

                if (sc.Degrade())
                {
                    Map[sc.X, sc.Y].SenseData.Remove(sc);
                    currentLevel.SenseData.Remove(sc);
                }
                else
                {
                    foreach (Scent n in sc.Propogate().OrderByDescending(n => n.ScentValue))
                    {
                        if (!currentLevel.SenseData.Any(s => s.X == n.X && s.Y == n.Y))
                        {
                            //the propogated scent item is not on the map
                            Map[n.X, n.Y].SenseData.Add(n);
                            currentLevel.SenseData.Add(n);

                        }
                        else
                        {
                            //the cell examined has scent data
                            var es = Map[n.X, n.Y].SenseData.First() as Scent;
                            if (es.ScentValue < n.ScentValue)
                            {
                                es.ScentValue = n.ScentValue;
                            }
                        }
                    }
                }
            }
        }




        #region mouse / key handling


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
                        , "MXL : " + (MaxLevel+1).ToString()
                    }
                );
        }

        #endregion




    }
}
