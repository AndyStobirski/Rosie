using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rosie.Entities;
using Rosie.Enums;
using Rosie.Map;
using System;

namespace Rosie.Draw
{

    /// <summary>
    /// Draw the game data on the screen
    /// </summary>
    public class DrawMap
    {
        public Point MousePos { get; set; }
        public SpriteFont Font { get; set; }

        /// <summary>
        /// Determine what to draw
        /// </summary>
        public GameViewMode ViewMode = GameViewMode.Game;

        /// <summary>
        /// The Player
        /// </summary>
        public Actor _player { get; set; }

        /// <summary>
        /// Player vision is calculated here
        /// </summary>
        FOVRecurse _fov;

        /// <summary>
        /// The size of the gameview, which a portion of the map centered
        /// arround the player
        /// </summary>
        public Point GameViewSize { get; set; } = new Point(30, 30);

        /// <summary>
        /// The offset to start drawing the game view based on the top left
        /// hand of the ganme 
        /// </summary>
        Point GameViewOffset { get; set; } = new Point(32, 32);

        /// <summary>
        /// The rectangle which defines the GameView on the map, recalculate every time
        /// the player moves
        /// </summary>
        private Rectangle _gameViewDefinition;
        public Rectangle GameViewDefinition => _gameViewDefinition;

        public enum Dungeon
        {
            Wall = 2
                , Floor = 30
        }
        private SpriteBatch _spriteBatch;

        public Point TileSize { get; } = new Point(32, 32);

        private Texture2D _dungeonTx;   //brickwall: 2, floor:30, stairsdown:57, stairsup:58, dooropen:47, doorclosed:48 
        private Texture2D _monsterTx;   //skeleton:338, ghoul: 205, sheep: 216
        private Texture2D _playerTx;    //playerbase: 0
        private Texture2D _itemTx;      //sword:0, armour:114, wand:174, food:186, ring:210, pendat:239, patient:268, spellbook:282
        private Texture2D _bubbleTx;
        private readonly Point _dungeonSize;
        private readonly Point _monsterSize;
        private readonly Point _playerSize;
        private readonly Point _itemSize;
        private readonly Point _bubbleSize;

        private int MapSizeWidth { get { return _CurrentLevel.Map.GetLength(0); } }
        private int MapSizeHeight { get { return _CurrentLevel.Map.GetLength(1); } }

        public CurrentLevel _CurrentLevel { get; set; }

        /// <summary>
        /// Recalculate the GameViewDefinition based on the Player location and then
        /// calculate the visible cells
        /// </summary>
        public void PlayerMove()
        {

            // GameView
            _gameViewDefinition.X = _player.X - GameViewSize.X / 2;
            _gameViewDefinition.Y = _player.Y - GameViewSize.Y / 2;

            if (_gameViewDefinition.X < 0)
                _gameViewDefinition.X = 0;
            else if (_gameViewDefinition.X + GameViewSize.X > MapSizeWidth)
                _gameViewDefinition.X -= (_gameViewDefinition.X + GameViewSize.X - MapSizeWidth);

            if (_gameViewDefinition.Y < 0)
                _gameViewDefinition.Y = 0;
            else if (_gameViewDefinition.Y + GameViewSize.Y > MapSizeHeight)
                _gameViewDefinition.Y -= (_gameViewDefinition.Y + GameViewSize.Y - MapSizeHeight);

            _gameViewDefinition.Width = GameViewSize.X;
            if (_gameViewDefinition.Right > MapSizeWidth)
                _gameViewDefinition.Width -= (_gameViewDefinition.Right - MapSizeWidth);

            _gameViewDefinition.Height = GameViewSize.Y;
            if (_gameViewDefinition.Bottom > MapSizeHeight)
                _gameViewDefinition.Height -= (_gameViewDefinition.Bottom - MapSizeHeight);


            //Calculate visible cells
            _fov.GetVisibleCells(_gameViewDefinition);
        }

        public DrawMap(Texture2D _pDungeonTx, Texture2D pMonsterTx, Texture2D pPlayerTx, Texture2D pItemTx, Texture2D pBubble, SpriteBatch pSpriteBatch, Actor pPlayer)
        {
            _dungeonTx = _pDungeonTx;
            _dungeonSize = new Point(_dungeonTx.Width / TileSize.X, _dungeonTx.Height / TileSize.Y);

            _monsterTx = pMonsterTx;
            _monsterSize = new Point(_monsterTx.Width / TileSize.X, _monsterTx.Height / TileSize.Y);

            _playerTx = pPlayerTx;
            _playerSize = new Point(_playerTx.Width / TileSize.X, _playerTx.Height / TileSize.Y);

            _itemTx = pItemTx;
            _itemSize = new Point(pItemTx.Width / TileSize.X, pItemTx.Height / TileSize.Y);

            _bubbleTx = pBubble;
            _bubbleSize = new Point(_bubbleTx.Width, _bubbleTx.Height);

            _spriteBatch = pSpriteBatch;

            _player = pPlayer;

            
        }

        public void SetCurrentLevel (CurrentLevel pCurrentLevel)
        {
            _CurrentLevel = pCurrentLevel;
            _fov = new FOVRecurse(_player, _CurrentLevel.Map);

        }

        private void DrawGame()
        {

            bool visible;

            for (int x = _gameViewDefinition.Left; x < _gameViewDefinition.Right; x++)
            {
                for (int y = _gameViewDefinition.Top; y < _gameViewDefinition.Bottom; y++)
                {

                    //
                    //  draw black tile
                    //
                    DrawDungeonTile(
                    1
                    , new Rectangle(
                                        (x - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
                                        , (y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
                                        , TileSize.X
                                        , TileSize.Y
                                     )
                    , Color.White
                    );

                    //  lookup whether the player can see the tile
                    visible = _fov.GameViewVisibilityGrid[x - _gameViewDefinition.Left, y - _gameViewDefinition.Top];

                    
                    if (_CurrentLevel.Map[x, y]?.Viewed == 0 && visible)
                        _CurrentLevel.Map[x, y].Viewed = 1 ;

                    if (_CurrentLevel.Map[x, y] != null)
                        if (_CurrentLevel.Map[x,y].Viewed > 0)
                    {

                        //draw the bFloor in the specified point
                        DrawDungeonTile(
                            (int)DrawMap.Dungeon.Floor
                            , new Rectangle(
                                                (x - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
                                                , (y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
                                                , TileSize.X
                                                , TileSize.Y
                                             )
                            , visible ? Color.White : Color.DarkSlateGray
                            );

                            if (visible && _CurrentLevel.Map[x, y].Inhabitant is Monster)
                            {
                                Monster monster = _CurrentLevel.Map[x, y].Inhabitant as Monster;

                                var monsterRect = new Rectangle(
                                 (monster.X - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
                                 , (monster.Y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
                                 , TileSize.X
                                 , TileSize.Y
                              );

                                GetMonsterTile(monster.Gfx, monsterRect);

                                string behaviour = Enum.GetName(typeof(MonsterBehaviour), monster.Behaviour); /*+ $" {monster.X},{monster.Y}->{monster.RoamTarget.X}, {monster.RoamTarget.Y}";*/

                                _spriteBatch.DrawString(Font, behaviour, new Vector2(monsterRect.X, monsterRect.Y - 10), Color.White);
                            }
                    }
                        //else
                        //{
                        //    DrawDungeonTile(
                        //    1
                        //    , new Rectangle(
                        //                        (x - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
                        //                        , (y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
                        //                        , TileSize.X
                        //                        , TileSize.Y
                        //                     )
                        //    , Color.White
                        //    );
                        //}
                }


                //draw mouse reticule
                DrawDungeonTile(
                    74
                        , new Rectangle(
                            MousePos.X * TileSize.X + GameViewOffset.X
                            , MousePos.Y * TileSize.Y + GameViewOffset.Y
                            , TileSize.X
                            , TileSize.Y
                         )
                , Color.White
                );
            }

            //
            //  DEBUG CODE: draw way points
            //
            //foreach (var wp in _CurrentLevel.wayPoints)
            //{
            //    if (_gameViewDefinition.Contains(wp.X, wp.Y))
            //    {
            //        DrawDungeonTile(
            //            35
            //            , new Rectangle(
            //                (wp.X - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
            //                , (wp.Y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
            //                , TileSize.X
            //                , TileSize.Y
            //                )
            //            , Color.White
            //            );
            //    }
            //}
            //
            //
            //

            DrawerPlayerTile(_player.Gfx, new Rectangle(
                                                (_player.X - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
                                                , (_player.Y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
                                                , TileSize.X
                                                , TileSize.Y
                                             ));

            //foreach (var monster in _CurrentLevel.Monsters)
            //{

            //    if (_gameViewDefinition.Contains(monster.X, monster.Y))
            //    {
            //        var monsterRect = new Rectangle(
            //                                        (monster.X - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
            //                                        , (monster.Y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y
            //                                        , TileSize.X
            //                                        , TileSize.Y
            //                                     );

            //        GetMonsterTile(monster.Gfx, monsterRect);

            //        string behaviour = Enum.GetName(typeof(MonsterBehaviour), monster.IamDoing); /*+ $" {monster.X},{monster.Y}->{monster.RoamTarget.X}, {monster.RoamTarget.Y}";*/

            //        _spriteBatch.DrawString(Font, behaviour, new Vector2(monsterRect.X, monsterRect.Y - 10), Color.White);

            //        //DrawerThoughtBubble(
            //        //    (monster.X - _gameViewDefinition.Left) * TileSize.X + GameViewOffset.X
            //        //    , (monster.Y - _gameViewDefinition.Top) * TileSize.Y + GameViewOffset.Y - _bubbleSize.Y
            //        //    );
            //    }
            //}

        }

        private void DrawMiniMap()
        {
            int size = 4;

            for (int x = 0; x < MapSizeWidth; x++)
                for (int y = 0; y < MapSizeHeight; y++)
                    if (_CurrentLevel.Map[x, y] != null)
                    {
                        DrawDungeonTile(
                             (int)DrawMap.Dungeon.Floor
                             , new Rectangle(
                                                 x * size + GameViewOffset.X
                                                 , y * size + GameViewOffset.Y
                                                 , size
                                                 , size
                                              )
                             , Color.White
                             );
                    }
                    else
                    {
                        DrawDungeonTile(
        1
        , new Rectangle(
                            x * size + GameViewOffset.X
                            , y * size + GameViewOffset.Y
                            , size
                            , size
                         )
        , Color.White
        );
                    }

            DrawerPlayerTile(_player.Gfx, new Rectangle(
                                                _player.X * size + GameViewOffset.X
                                                , _player.Y * size + GameViewOffset.Y
                                                , size
                                                , size
                                             ));

        }

        public void Draw()
        {
            switch (ViewMode)
            {
                case GameViewMode.Game:
                    DrawGame();
                    break;

                case GameViewMode.MiniMap:
                    DrawMiniMap();
                    break;
            }
        }


        private Rectangle GetRectangle(int pTile, Point pSize)
        {
            int y = (pTile / pSize.X);
            int x = pTile - y * pSize.X;
            return new Rectangle(x * TileSize.X, y * TileSize.Y, TileSize.X, TileSize.Y);

        }


        //Tile numbering starts at 0 in the top left hand corner
        public void DrawDungeonTile(int pTile, Rectangle pDest, Color pBackColour)
        {

            _spriteBatch.Draw(_dungeonTx, pDest, GetRectangle(pTile, _dungeonSize), pBackColour);

        }

        public void GetMonsterTile(int pTile, Rectangle pDest)
        {
            _spriteBatch.Draw(_monsterTx, pDest, GetRectangle(pTile, _monsterSize), Color.White);
        }

        public void DrawerPlayerTile(int pTile, Rectangle pDest)
        {

            _spriteBatch.Draw(_playerTx, pDest, GetRectangle(pTile, _playerSize), Color.White);

        }

        public void DrawerThoughtBubble(int pX, int pY)
        {

            _spriteBatch.Draw(_bubbleTx, new Rectangle(pX, pY, _bubbleSize.X, _bubbleSize.Y), new Rectangle(0,0, _bubbleSize.X, _bubbleSize.Y), Color.White);

        }

        public void Update()
        {

        }

    }
}
