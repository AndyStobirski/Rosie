using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rosie.Code;
using Rosie.Entities;
using Rosie.Enums;
using Rosie.Misc;
using System;
using System.Diagnostics;
using System.Linq;

namespace Rosie
{
    public class Game1 : Game
    {
        //  MonoGame
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly FrameCounter _frameCounter = new FrameCounter();
        SpriteFont _font;
        InputHandler _InputHandler;
        Texture2D pixel;

        private Texture2D _tiles;

        RosieGame _Rosie;

        //  Monitor user input
        KeyboardState newKeyboardState;
        KeyboardState oldKeyboardState;
        MouseState newMouseState;
        MouseState oldMouseState;



        public Game1()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //Hardcode the frame rate
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d);

        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.ApplyChanges();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            _InputHandler = new InputHandler();
            _InputHandler.GameCommandIssued += _InputHandler_GameCommandIssued;

            base.Initialize();
        }

        /// <summary>
        /// The input handler has completed a 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _InputHandler_GameCommandIssued(object sender, InputHandler.GameCommandEventArgs e)
        {
            //isue command to game
            _Rosie.GameCommand(e.Command, e.Data);

        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Pixel");
            _Rosie = new RosieGame();
            _tiles = Content.Load<Texture2D>("rltiles-2d");
        }




        protected override void Update(GameTime gameTime)
        {
            newKeyboardState = Keyboard.GetState();
            newMouseState = Mouse.GetState();
            var mouseState = Mouse.GetState();


            //Check if mouse inside the camera area
            //and set the position based on tile size
            if (_Rosie.CameraBorder.Contains(mouseState.X, mouseState.Y))
            {
                RosieGame.SelectedTile = new Point(
                    ((mouseState.X - _Rosie.GameCameraOffset.X) / _Rosie.TileSize.Width) + _Rosie.GameCameraDefinition.X,
                    ((mouseState.Y - _Rosie.GameCameraOffset.Y) / _Rosie.TileSize.Height) + _Rosie.GameCameraDefinition.Y
                    );

            }

            _Rosie.ShiftDown = newKeyboardState.IsKeyDown(Keys.LeftShift) || newKeyboardState.IsKeyDown(Keys.RightShift);
            _Rosie.NumLock = newKeyboardState.NumLock;
            _Rosie.CapsLock = newKeyboardState.CapsLock;

            //  Detect left mouse click
            if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
            {
                _Rosie.MouseClick(mouseState.X, mouseState.Y);
            }

            if (_Rosie.GameState == Enums.GameStates.PlayerTurn)
            {

                //
                //  Detect keyup
                //
                foreach (var key in from Keys key in oldKeyboardState.GetPressedKeys()
                        .Where(k => newKeyboardState.IsKeyUp(k))
                                    where newKeyboardState.IsKeyUp(key)
                                    select key)
                {

                    Debug.WriteLine(String.Format("{0} - {1}", key, (int)key));


                    _InputHandler.ProcessCommand((int)key);

                    // _Rosie.KeyPress((int)key);

                }

            }
            else if (_Rosie.GameState == Enums.GameStates.EnemyTurn)
                _Rosie.Tick();



            base.Update(gameTime);

            oldKeyboardState = newKeyboardState;
            oldMouseState = newMouseState;
        }



        /// <summary>
        /// Draw game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            if (RosieGame.currentLevel == null) return;

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();


            #region Draw debug code



            #endregion


            switch (RosieGame.ViewMode)
            {
                case Enums.GameViewMode.Game:
                    /*
                        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        _frameCounter.Update(deltaTime);
                        var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
                       _spriteBatch.DrawString(_font, fps, new Vector2(1, 1), Color.White);
                    */

                    _spriteBatch.DrawString(_font, string.Format("CAPS: {0}, NUM: {1}, SHIFT: {2}", _Rosie.CapsLock, _Rosie.NumLock, _Rosie.ShiftDown), new Vector2(_graphics.PreferredBackBufferWidth - 300, 1), Color.White);



                    DrawGame();
                    DrawMapFrame();
                    DrawGameMessages();
                    DrawPlayerStats();
                    break;

                case Enums.GameViewMode.MiniMap:
                    DrawMiniMap();
                    break;

                case Enums.GameViewMode.IO:
                    DrawInventory();
                    break;

            }



            _spriteBatch.End();

            base.Draw(gameTime);
        }




        protected void DrawPlayerStats()
        {
            Point drawOrigin = new Point(10, 50);
            _spriteBatch.DrawString(_font, RosieGame.player.Properties() + "\r\nTNS: " + _Rosie.TurnCounter, new Vector2(drawOrigin.X, drawOrigin.Y), Color.White);
        }




        #region Map Drawing Code


        /// <summary>
        /// Convert an index of image to it's location on the texture _tiles
        /// </summary>
        /// <param name="pIndex">U</param>
        /// <returns></returns>
        private Rectangle IndexToRectangle(int pIndex)
        {

            Size _tilesSize = new Size(_tiles.Width / 32, _tiles.Height / 32);
            int y = (pIndex / _tilesSize.Width);
            int x = pIndex - y * _tilesSize.Width;

            return new Rectangle(x * 32, y * 32, 32, 32);
        }

        public void DrawTile(int pTile, Rectangle pDest, Color pBackColour)
        {
            _spriteBatch.Draw(_tiles, pDest, IndexToRectangle(pTile), pBackColour);
        }

        private void DrawMiniMap()
        {
            int size = 4;

            for (int x = 0; x < _Rosie.MapSize.Width; x++)
                for (int y = 0; y < _Rosie.MapSize.Height; y++)
                    if (_Rosie.Map[x, y] != null)
                    {
                        DrawTile(
                             1378
                             , new Rectangle(
                                                 x * size + _Rosie.GameCameraOffset.X
                                                 , y * size + _Rosie.GameCameraOffset.Y
                                                 , size
                                                 , size
                                              )
                             , Color.White
                             );
                    }
                    else
                    {
                        DrawTile(
                            1649
                            , new Rectangle(
                                                x * size + _Rosie.GameCameraOffset.X
                                                , y * size + _Rosie.GameCameraOffset.Y
                                                , size
                                                , size
                                             )
                            , Color.White
                            );
                    }

            DrawTile(RosieGame.player.Gfx, new Rectangle(
                                            RosieGame.player.X * size + _Rosie.GameCameraOffset.X
                                            , RosieGame.player.Y * size + _Rosie.GameCameraOffset.Y
                                            , size
                                            , size
                                         )
             , Color.White
            );

        }

        /// <summary>
        /// Draw a frame round round the GameView
        /// </summary>
        protected void DrawMapFrame()
        {

            Color col = Color.White;

            //top line
            _spriteBatch.Draw(pixel
                , new Rectangle(_Rosie.CameraBorder.X
                    , _Rosie.CameraBorder.Y
                    , _Rosie.CameraBorder.Width
                    , 1)
                , col);

            //bottom line
            _spriteBatch.Draw(pixel
                , new Rectangle(_Rosie.CameraBorder.X
                    , _Rosie.CameraBorder.Y + _Rosie.CameraBorder.Height
                    , _Rosie.CameraBorder.Width
                    , 1)
                , col);

            //left line
            _spriteBatch.Draw(pixel
                , new Rectangle(_Rosie.CameraBorder.X
                    , _Rosie.CameraBorder.Y
                    , 1
                    , _Rosie.CameraBorder.Height)
                , col);

            //right line
            _spriteBatch.Draw(pixel
                , new Rectangle(_Rosie.CameraBorder.Width + _Rosie.CameraBorder.X
                    , _Rosie.GameCameraOffset.Y
                    , 1
                    , _Rosie.CameraBorder.Height)
                , col);



            //
            //
            //
            _spriteBatch.DrawString(_font, string.Format("Mouse: {0},{1}", RosieGame.SelectedTile.X, RosieGame.SelectedTile.Y), new Vector2(_Rosie.CameraBorder.X, _Rosie.CameraBorder.Y), Color.White);

        }

        #endregion

        #region GameDrawing Code
        private void DrawGame()
        {

            bool visible;

            for (int x = _Rosie.GameCameraDefinition.Left; x < _Rosie.GameCameraDefinition.Right; x++)
            {
                for (int y = _Rosie.GameCameraDefinition.Top; y < _Rosie.GameCameraDefinition.Bottom; y++)
                {
                    //  lookup whether the player can see the tile
                    visible = _Rosie._fov.GameViewVisibilityGrid[x - _Rosie.GameCameraDefinition.Left, y - _Rosie.GameCameraDefinition.Top];

                    //the tile being examined
                    var tile = _Rosie.Map[x, y];

                    //the rectangle to draw into
                    var rect = new Rectangle(
                         (x - _Rosie.GameCameraDefinition.Left) * _Rosie.TileSize.Width + _Rosie.GameCameraOffset.X
                         , (y - _Rosie.GameCameraDefinition.Top) * _Rosie.TileSize.Height + _Rosie.GameCameraOffset.Y
                         , _Rosie.TileSize.Width
                         , _Rosie.TileSize.Height);


                    if (tile?.Viewed == 0 && visible)
                        tile.Viewed = 1;

                    if (tile != null && tile.Viewed > 0)
                    {

                        //draw the bFloor in the specified point
                        DrawTile(
                            tile.Gfx
                            , rect
                            , visible ? Color.White : Color.DarkSlateGray
                            );

                        // Draw the topmost item
                        if (visible && tile.Items.Any())
                        {
                            DrawTile(
                                tile.Items.Last().Gfx
                                , rect
                                , Color.White
                                );
                        }

                        // Draw Monster / Player
                        if ((tile.Inhabitant is Player) || (visible && tile.Inhabitant is NPC))
                        {
                            DrawTile(tile.Inhabitant.Gfx, rect, Color.White);

                            DrawHitpoints(
                                rect
                                , tile.Inhabitant.HitPointsCurrent
                                , tile.Inhabitant.HitPointsMax
                                );

                            if (tile.Inhabitant is NPC)
                            {
                                DrawNPCState(rect, (tile.Inhabitant as NPC).script.State);
                            }
                        }
                    }
                }


                //RosieGame.SelectedTile is the currently selected tile
                //but in order to draw it on the camera view we need to translate it

                Point p = new Point(RosieGame.SelectedTile.X, RosieGame.SelectedTile.Y);
                p.X -= _Rosie.GameCameraDefinition.X;
                p.Y -= _Rosie.GameCameraDefinition.Y;

                //draw mouse reticule
                DrawTile(
                    (int)GFXValues.MOUSE_RETICULE
                        , new Rectangle(
                            _Rosie.GameCameraOffset.X + (p.X * _Rosie.TileSize.Width)
                            , _Rosie.GameCameraOffset.Y + (p.Y * _Rosie.TileSize.Height)
                            , _Rosie.TileSize.Width
                            , _Rosie.TileSize.Height
                         )
                , Color.White
                );
            }

            // draw hitpoints of monsters / players
            // we do this last as it we can esnure any data is on top and not
            // drawn over by other processes./


        }

        /// <summary>
        /// Draw the Actor's hitpoint bar
        /// </summary>
        /// <param name="pTargetRect"></param>
        /// <param name="pCurrent"></param>
        /// <param name="pMax"></param>
        protected void DrawHitpoints(Rectangle pTargetRect, int pCurrent, int pMax)
        {

            if (pCurrent == pMax)
                return;

            //  Draw maxhitpoints
            int barHeight = 3;


            if (pCurrent >= 0)
            {
                int current = (int)(((pCurrent * 1.0) / pMax) * _Rosie.TileSize.Width);

                //max hit points
                _spriteBatch.Draw(pixel, new Rectangle(pTargetRect.X, pTargetRect.Y - barHeight, _Rosie.TileSize.Width, barHeight), Color.Red);

                //current hitpoints
                _spriteBatch.Draw(pixel, new Rectangle(pTargetRect.X, pTargetRect.Y - barHeight, current, barHeight), Color.Green);


            }
        }

        protected void DrawNPCState(Rectangle pTargetRect, NPC_STATE State)
        {
            var gfx = (int)State + 1645;

            pTargetRect.Y -= 8;

            DrawTile(gfx, pTargetRect, Color.White);
        }

        #endregion


        protected void DrawGameMessages()
        {
            int drawMessageStart = _Rosie.GameCameraOffset.Y + _Rosie.GameCameraSize.Y * _Rosie.TileSize.Height;
            int ctr = 0;
            foreach (string message in RosieGame.Messages.Take(15))
            {
                _spriteBatch.DrawString(_font, message, new Vector2(1, drawMessageStart + ctr++ * 15), Color.White);
            }
        }

        #region Draw Inventory


        protected void DrawInventory()
        {
            Point drawOrigin = new Point(10, 10);


            _spriteBatch.DrawString(_font, _InputHandler.DisplayMessage, new Vector2(drawOrigin.X, drawOrigin.Y), Color.White);
        }

        #endregion

    }
}
