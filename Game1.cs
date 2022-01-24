using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rosie.Draw;
using Rosie.Entities;
using Rosie.Map;
using Rosie.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rosie
{
    public class Game1 : Game
    {
        //  MonoGame
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        //  game code
        DrawMap _DrawMap;
        CurrentLevel _CurrentLevel;
        Player _player;


        SpriteFont _font;
        private FrameCounter _frameCounter = new FrameCounter();



        List<AnimationEffect> _animates = new List<AnimationEffect>();

        //Timer _MonsterTimer = new Timer(500.0);



        //  Monitor user input
        KeyboardState newKeyboardState;
        KeyboardState oldKeyboardState;
        MouseState newMouseState;
        MouseState oldMouseState;


        /// <summary>
        /// The Coordinates of the mouse pointer, unadjusted
        /// </summary>
        Point _MousePosUnadjusted;

        private Point _MousePos => new Point(_MousePosUnadjusted.X + _DrawMap.GameViewDefinition.X, _MousePosUnadjusted.Y + _DrawMap.GameViewDefinition.Y);

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
            // TODO: Add your initialization logic here
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();



            base.Initialize();

        }

        /// <summary>
        /// The player has moved, so begin the update process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _player_PlayerMoved(object sender, System.EventArgs e)
        {

            _DrawMap.PlayerMove();

            //setting this marks the turn as over and we go looking for monsters to move
            _CurrentLevel.GameState = Enums.GameStates.EnemyTurn;

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font");
            _player = new Player();
            _player.ActorMoved += _player_PlayerMoved;

            _DrawMap = new DrawMap(
                    Content.Load<Texture2D>("dc-dngn")
                    , Content.Load<Texture2D>("dc-mon")
                    , Content.Load<Texture2D>("dc-pl")
                    , Content.Load<Texture2D>("dc-item")
                    , Content.Load<Texture2D>("bubble")
                    , _spriteBatch
                    , _player)
            {
                Font = _font
            };

            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            var cg = new MapGenerator()
            {
                Room_Min = new Size(3, 3)
                ,
                Room_Max = new Size(3, 3)
                ,
                MaxRooms = 10
                ,
                MapSize = new Size(30, 30)
                ,
                RoomDistance = 3
                ,
                Corridor_Max = 6
                ,
                Corridor_MaxTurns = 3
            };

            //_CurrentLevel = cg.Build();
            // var cg = new CustomGenerator();



            _CurrentLevel = cg.Build();
            _CurrentLevel.InitMonsters(_player, 5); //set the max number of monsters


            _DrawMap.SetCurrentLevel(_CurrentLevel);
            var pt = cg.GetStartLocation();
            _player.X = pt.X;
            _player.Y = pt.Y;
            _player.VisionRange = 8;

            _font = Content.Load<SpriteFont>("Font");

            _player.ActorMoved += _CurrentLevel.M_ActorMoved;

            _CurrentLevel.GameState = Enums.GameStates.PlayerTurn;

            _player_PlayerMoved(null, null);
        }

        protected override void Update(GameTime gameTime)
        {
            newKeyboardState = Keyboard.GetState();
            newMouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            _MousePosUnadjusted = _DrawMap.MousePos = new Point((mouseState.X - 32) / 32, (mouseState.Y - 32) / 32);


            if (_CurrentLevel.GameState == Enums.GameStates.PlayerTurn)
                PlayerMove();
            else if (_CurrentLevel.GameState == Enums.GameStates.EnemyTurn)
                _CurrentLevel.Tick();


            if (LMBClick())
            {
                //_animates.Add(new AnimationEffect(_font, _roller.Roll("2d4").ToString(), mouseState.X, mouseState.Y));
                //Trace.WriteLine("");
            }





            base.Update(gameTime);

            oldKeyboardState = newKeyboardState;
            oldMouseState = newMouseState;
        }



        /// <summary>
        /// Monitor player keypresses
        /// </summary>
        private void PlayerMove()
        {
            //_MonsterTimer.Wait(gameTime, () =>
            //{
            //    // do my timer based code...

            //    //_animates.Add(new Animate(_font, _roller.Roll("2d4").ToString(), _rnd.Next(0, 500), _rnd.Next(0, 500)));

            //    _CurrentLevel.DoMonster();

            //});

            if (newKeyboardState.IsKeyDown(Keys.Space))
            {
                _player.Move(0, 0);
            }

            if (KeypressTest(Keys.Up, Keys.NumPad8))
            {
                if (_CurrentLevel.IsWalkable(_player.X, _player.Y - 1))
                    _player.Move(0, -1);
            }

            if (KeypressTest(Keys.Down, Keys.NumPad2))
            {
                if (_CurrentLevel.IsWalkable(_player.X, _player.Y + 1))
                    _player.Move(0, 1);
            }

            if (KeypressTest(Keys.Left, Keys.NumPad4))
            {
                if (_CurrentLevel.IsWalkable(_player.X - 1, _player.Y))
                    _player.Move(-1, 0);
            }

            if (KeypressTest(Keys.Right, Keys.NumPad6))
            {
                if (_CurrentLevel.IsWalkable(_player.X + 1, _player.Y))
                    _player.Move(1, 0);
            }

            if (KeypressTest(Keys.NumPad7))
            {
                if (_CurrentLevel.IsWalkable(_player.X - 1, _player.Y - 1))
                    _player.Move(-1, -1);
            }

            if (KeypressTest(Keys.NumPad9))
            {
                if (_CurrentLevel.IsWalkable(_player.X + 1, _player.Y - 1))
                    _player.Move(1, -1);
            }

            if (KeypressTest(Keys.NumPad1))
            {
                if (_CurrentLevel.IsWalkable(_player.X - 1, _player.Y + 1))
                    _player.Move(-1, 1);
            }

            if (KeypressTest(Keys.NumPad3))
            {
                if (_CurrentLevel.IsWalkable(_player.X + 1, _player.Y + 1))
                    _player.Move(1, 1);
            }


            if (KeypressTest(Keys.M))
            {
                if (_DrawMap.ViewMode == Enums.GameViewMode.Game)
                    _DrawMap.ViewMode = Enums.GameViewMode.MiniMap;
                else
                    _DrawMap.ViewMode = Enums.GameViewMode.Game;
            }

            //Output map
            if (KeypressTest(Keys.O))
            {
                _CurrentLevel.OutputMap();
                _animates.Add(new AnimationEffect(_font, "Map outputted", 500, 500));
            }


            if (KeypressTest(Keys.F1))
            {
                LoadCurrentLevel();
            }



        }


        /// <summary>
        /// Test for a single keypress
        /// </summary>
        /// <param name="key">The key to be tested
        /// <returns></returns>
        private bool KeypressTest(params Keys[] pPressed)
        {
            foreach (Keys key in pPressed)
                if (newKeyboardState.IsKeyUp(key) && oldKeyboardState.IsKeyDown(key))
                {
                    return true;
                }

            return false;
        }

        private bool LMBClick()
        {
            return newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed;
        }


        /// <summary>
        /// Draw game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            if (_CurrentLevel == null) return;

            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();

            #region Draw debug code

            if (_MousePos.X >= 0 && _MousePos.X < _CurrentLevel.Map.GetLength(0) && _MousePos.Y >= 0 && _MousePos.Y < _CurrentLevel.Map.GetLength(1))
                _spriteBatch.DrawString(_font, $"Mouse:{_MousePos.X}, {_MousePos.Y}, Walkable:{_CurrentLevel.Map[_MousePos.X, _MousePos.Y]?.Walkable()}, Viewed:{_CurrentLevel.Map[_MousePos.X, _MousePos.Y]?.Viewed}", new Vector2(250, 1), Color.Black);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
            _spriteBatch.DrawString(_font, fps, new Vector2(1, 1), Color.Black);

            _spriteBatch.DrawString(_font, string.Format("Player: {0},{1}", _player.X, _player.Y), new Vector2(1, 15), Color.Black);

            #endregion

            _DrawMap.Draw();



            //
            //   Iterate the items in the animation queue
            //
            foreach (var a in _animates)
            {
                a.Tick();
                a.Draw(_spriteBatch);
            }

            _animates.RemoveAll(a => a.Remove);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
