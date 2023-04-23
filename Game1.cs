using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rosie.Code;
using Rosie.Code.Misc;
using Rosie.Code.sensedata;
using Rosie.Entities;
using Rosie.Misc;
using System;
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

        /// <summary>
        /// The map cell the mouse pointer is over
        /// </summary>
        Point _MouseOverMapCell;

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

            RandomWithSeed.SetSeed("ifov-wfum-ougt");

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




            _Rosie.ShiftDown = newKeyboardState.IsKeyDown(Keys.LeftShift) || newKeyboardState.IsKeyDown(Keys.RightShift);
            _Rosie.NumLock = newKeyboardState.NumLock;
            _Rosie.CapsLock = newKeyboardState.CapsLock;

            //  Detect left mouse click
            if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
            {
                _Rosie.MouseClick(mouseState.X, mouseState.Y);
            }


            //Check if mouse inside the camera area
            //and set the position based on tile size
            if (Camera.CameraBorder.Contains(mouseState.X, mouseState.Y))
            {
                _MouseOverMapCell = new Point(
                    (mouseState.X - Camera.GameCameraOffset.X) / Camera.TileSize.Width + Camera.GameCameraDefinition.X,
                    (mouseState.Y - Camera.GameCameraOffset.Y) / Camera.TileSize.Height + Camera.GameCameraDefinition.Y
                    );

            }
            else
            {
                _MouseOverMapCell = new Point(-1, -1);
            }





            if (_Rosie.GameState == GameStates.PlayerTurn)
            {


                //
                //  Detect mouse click in game view
                //
                if (ClickInGameView(mouseState.X, mouseState.Y, out Point pClick))
                {
                    _InputHandler.MapCellClicked(_MouseOverMapCell.X, _MouseOverMapCell.Y);
                }


                //
                //  Detect keyup
                //
                foreach (var key in from Keys key in oldKeyboardState.GetPressedKeys()
                        .Where(k => newKeyboardState.IsKeyUp(k))
                                    where newKeyboardState.IsKeyUp(key)
                                    select key)
                {
                    _InputHandler.GetUserKeyPress((int)key);
                }

                if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    _Rosie.MouseClick(mouseState.X, mouseState.Y);
                }


            }
            else if (_Rosie.GameState == GameStates.EnemyTurn)
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


            switch (RosieGame.ViewMode)
            {
                case GameViewMode.Game:
                    var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _frameCounter.Update(deltaTime);
                    var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
                    _spriteBatch.DrawString(_font, fps, new Vector2(1, 1), Color.White);


                    _spriteBatch.DrawString(_font, string.Format("CAPS: {0}, NUM: {1}, SHIFT: {2}", _Rosie.CapsLock, _Rosie.NumLock, _Rosie.ShiftDown), new Vector2(_graphics.PreferredBackBufferWidth - 300, 1), Color.White);

                    DrawGame();
                    break;

                case GameViewMode.MiniMap:
                    DrawMiniMap();
                    break;

                case GameViewMode.InputHandlerMode:

                    DrawGame();
                    DrawInputHandler();
                    break;

            }



            _spriteBatch.End();

            base.Draw(gameTime);
        }




        protected void DrawPlayerStats()
        {
            Point drawOrigin = new Point(10, 50);
            _spriteBatch.DrawString(_font, _Rosie.PlayerProperties() + "\r\nTNS: " + _Rosie.TurnCounter, new Vector2(drawOrigin.X, drawOrigin.Y), Color.White);
        }


        /// <summary>
        /// Check if a mouse click in the game camera has taken place
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pClick"></param>
        /// <returns></returns>
        private bool ClickInGameView(int pX, int pY, out Point pClick)
        {
            pClick = Point.Zero;

            //Check if mouse inside the camera area
            //and set the position based on tile size
            if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed && _MouseOverMapCell.X != -1 && _MouseOverMapCell.Y != -1)
            {
                return true;
            }
            else
                return false;
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
            int y = pIndex / _tilesSize.Width;
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

            for (int x = 0; x < Camera.MapSize.Width; x++)
                for (int y = 0; y < Camera.MapSize.Height; y++)
                    if (_Rosie.Map[x, y] != null)
                    {
                        DrawTile(
                             1378
                             , new Rectangle(
                                                 x * size + Camera.GameCameraOffset.X
                                                 , y * size + Camera.GameCameraOffset.Y
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
                                                x * size + Camera.GameCameraOffset.X
                                                , y * size + Camera.GameCameraOffset.Y
                                                , size
                                                , size
                                             )
                            , Color.White
                            );
                    }

            DrawTile(RosieGame.player.Gfx, new Rectangle(
                                            RosieGame.player.X * size + Camera.GameCameraOffset.X
                                            , RosieGame.player.Y * size + Camera.GameCameraOffset.Y
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
                , new Rectangle(Camera.CameraBorder.X
                    , Camera.CameraBorder.Y
                    , Camera.CameraBorder.Width
                    , 1)
                , col);

            //bottom line
            _spriteBatch.Draw(pixel
                , new Rectangle(Camera.CameraBorder.X
                    , Camera.CameraBorder.Y + Camera.CameraBorder.Height
                    , Camera.CameraBorder.Width
                    , 1)
                , col);

            //left line
            _spriteBatch.Draw(pixel
                , new Rectangle(Camera.CameraBorder.X
                    , Camera.CameraBorder.Y
                    , 1
                    , Camera.CameraBorder.Height)
                , col);

            //right line
            _spriteBatch.Draw(pixel
                , new Rectangle(Camera.CameraBorder.Width + Camera.CameraBorder.X
                    , Camera.GameCameraOffset.Y
                    , 1
                    , Camera.CameraBorder.Height)
                , col);



            //
            //  Draw mouse coordinates
            //
            _spriteBatch.DrawString(_font, string.Format("Mouse: {0},{1}", RosieGame.MouseOverTile.X, RosieGame.MouseOverTile.Y), new Vector2(Camera.CameraBorder.X, Camera.CameraBorder.Y), Color.White);

        }

        #endregion

        #region GameDrawing Code

        private void DrawGame()
        {
            DrawCameraView();
            DrawMapFrame();
            DrawGameMessages();
            DrawPlayerStats();
        }

        private void DrawCameraView()
        {

            bool visible;

            for (int x = Camera.GameCameraDefinition.Left; x < Camera.GameCameraDefinition.Right; x++)
            {
                for (int y = Camera.GameCameraDefinition.Top; y < Camera.GameCameraDefinition.Bottom; y++)
                {
                    //  lookup whether the player can see the tile
                    visible = _Rosie._fov.LightGrid[x - Camera.GameCameraDefinition.Left, y - Camera.GameCameraDefinition.Top] > 0;

                    //the tile being examined
                    var tile = _Rosie.Map[x, y];

                    //the rectangle to draw into
                    var rect = new Rectangle(
                         (x - Camera.GameCameraDefinition.Left) * Camera.TileSize.Width + Camera.GameCameraOffset.X
                         , (y - Camera.GameCameraDefinition.Top) * Camera.TileSize.Height + Camera.GameCameraOffset.Y
                         , Camera.TileSize.Width
                         , Camera.TileSize.Height);


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
                        if (tile.Inhabitant is Player || visible && tile.Inhabitant is NPC)
                        {
                            DrawTile(tile.Inhabitant.Gfx, rect, Color.White);

                            if (tile.Inhabitant is NPC)
                            {
                                DrawNPCState(rect, (tile.Inhabitant as NPC).script.State);

                                if ((tile.Inhabitant as NPC).script.State == NPC_STATE.Combat)
                                {
                                    DrawHitpoints(
                                      rect
                                      , tile.Inhabitant.HitPointsCurrent
                                      , tile.Inhabitant.HitPointsMax
                                      );
                                }

                            }
                        }

                        //draw sense data
                        if (_Rosie.DislayOdourCloud && tile.SenseData.Any())
                        {
                            var s = tile.SenseData.First() as Scent;

                            _spriteBatch.DrawString(_font, s.ScentValue.ToString(), new Vector2(rect.X, rect.Y), Color.White);
                        }
                    }
                }


                //RosieGame.SelectedTile is the currently selected tile
                //but in order to draw it on the camera view we need to translate it

                Point p = new Point(RosieGame.MouseOverTile.X, RosieGame.MouseOverTile.Y);
                p.X -= Camera.GameCameraDefinition.X;
                p.Y -= Camera.GameCameraDefinition.Y;

                //draw mouse reticule
                DrawTile(
                    (int)GFXValues.MOUSE_RETICULE
                        , new Rectangle(
                            Camera.GameCameraOffset.X + p.X * Camera.TileSize.Width
                            , Camera.GameCameraOffset.Y + p.Y * Camera.TileSize.Height
                            , Camera.TileSize.Width
                            , Camera.TileSize.Height
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
                int current = (int)(pCurrent * 1.0 / pMax * Camera.TileSize.Width);

                //max hit points
                _spriteBatch.Draw(pixel, new Rectangle(pTargetRect.X, pTargetRect.Y - barHeight, Camera.TileSize.Width, barHeight), Color.Red);

                //current hitpoints
                _spriteBatch.Draw(pixel, new Rectangle(pTargetRect.X, pTargetRect.Y - barHeight, current, barHeight), Color.Green);


            }
        }

        /// <summary>
        /// Draw an icon and healthbar over the head of the NPC 
        /// </summary>
        /// <param name="pTargetRect"></param>
        /// <param name="State"></param>
        protected void DrawNPCState(Rectangle pTargetRect, NPC_STATE State)
        {
            int gfx = 0;
            switch (State)
            {
                case NPC_STATE.Alert:
                    gfx = (int)GFXValues.MONSTER_STATE_ALERT;
                    break;
                case NPC_STATE.Combat:
                    gfx = (int)GFXValues.MONSTER_STATE_COMBAT;
                    break;
                case NPC_STATE.Exploring:
                    gfx = (int)GFXValues.MONSTER_STATE_EXPLORING;
                    break;
                case NPC_STATE.Sleeping:
                    gfx = (int)GFXValues.MONSTER_STATE_SLEEP;
                    break;
                case NPC_STATE.Idle:
                    gfx = (int)GFXValues.MONSTER_STATE_IDLE;
                    break;
                case NPC_STATE.TrackScent:
                    gfx = (int)GFXValues.MONSTER_STATE_TRACK_SCENT;
                    break;
            }

            pTargetRect.Y -= 8;

            DrawTile(gfx, pTargetRect, Color.White);
        }

        #endregion


        protected void DrawGameMessages()
        {
            int drawMessageStart = Camera.GameCameraOffset.Y + Camera.GameCameraSize.Y * Camera.TileSize.Height;
            int ctr = 0;
            foreach (string message in RosieGame.Messages.Take(15))
            {
                _spriteBatch.DrawString(_font, message, new Vector2(1, drawMessageStart + ctr++ * 15), Color.White);
            }
        }

        #region Draw Inventory



        /// <summary>
        /// Darw the messages from the input handler, a block at the top of the screen
        /// which is the height of the displayed text, with two white lines underneath it
        /// </summary>
        protected void DrawInputHandler()
        {
            Point drawOrigin = new Point(10, 10);

            //get dimensions of the text to be drawn
            Vector2 textSize = _font.MeasureString(_InputHandler.DisplayMessage);

            //draw black background
            _spriteBatch.Draw(pixel, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, (int)textSize.Y + drawOrigin.Y * 2), Color.Black);

            // draw the text
            _spriteBatch.DrawString(_font, _InputHandler.DisplayMessage, new Vector2(drawOrigin.X, drawOrigin.Y), Color.White);


            Color col = Color.White;

            //top line
            _spriteBatch.Draw(pixel
                , new Rectangle(0
                    , (int)textSize.Y + drawOrigin.Y * 2
                    , _graphics.PreferredBackBufferWidth
                    , 1)
                , col);

            //bottom line
            _spriteBatch.Draw(pixel
                , new Rectangle(0
                    , (int)textSize.Y + drawOrigin.Y * 2 - 2
                    , _graphics.PreferredBackBufferWidth
                    , 1)
                , col);


        }

        #endregion

    }
}
