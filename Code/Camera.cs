using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Entities;
using Rosie.Misc;


namespace Rosie.Code
{
    /// <summary>
    /// The camera is used to calculate an area, centred around the player, 
    /// which represents the main game view
    /// </summary>
    public static class Camera
    {

        public static Player player => RosieGame.player;
        public static Tile[,] Map => RosieGame.currentLevel.Map;

        /// <summary>
        /// The offset to start drawing the game view based on the top left
        /// hand of the ganme 
        /// </summary>
        public static Point GameCameraOffset { get; set; } = new Point(320, 32);

        /// <summary>
        /// The size of the tiles to draw
        /// </summary>
        public static Size TileSize { get; } = new Size(32, 32);

        /// <summary>
        /// The rectangle which defines the GameView on the map, recalculate every time the player moves
        /// </summary>
        private static Rectangle _GameCameraDefinition;
        public static Rectangle GameCameraDefinition => _GameCameraDefinition;

        /// <summary>
        /// The size of the gameview, which a portion of the map centered
        /// arround the player
        /// </summary>
        public static Point GameCameraSize { get; set; } = new Point(25, 25);


        /// <summary>
        /// Size of the GameMap in tiles
        /// </summary>
        public static Size MapSize => Map == null ? null : new Size(Map.GetLength(0), Map.GetLength(1));

        /// <summary>
        /// Defines the rectangle which contains the camera view over the map
        /// </summary>
        public static Rectangle CameraBorder => new Rectangle(GameCameraOffset.X, GameCameraOffset.Y, GameCameraSize.X * TileSize.Width, GameCameraSize.Y * TileSize.Height);

        /// <summary>
        /// Calculate the GameView, a rectangle which defines the drawn part of the map
        /// based on the location of the player
        /// </summary>
        public static void CalculateGameCameraDefinition()
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
        }
    }
}
