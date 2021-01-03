using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRexRunner.Entities;
using TRexRunner.System;

namespace TRexRunner
{
    public class TRexRunnerGame : Game
    {
        private const string ASSET_NAME_SPRITE_SHEET = "TrexSpriteSheet";
        private const string ASSET_NAME_SFX_HIT = "hit";
        private const string ASSET_NAME_SFX_SCORE_REACHED = "score-reached";
        private const string ASSET_NAME_SFX_BUTTON_PRESS = "button-press";

        public const int WINDOW_WIDTH = 600;
        public const int WINDOW_HEIGHT = 150;

        public const int TREX_START_POS_Y = WINDOW_HEIGHT - 16;
        public const int TREX_START_POS_X = 1;
        public const float FADE_IN_ANIMATION_SPEED = 820f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SoundEffect _sfxHit;
        private SoundEffect _sfxButtonPress;
        private SoundEffect _sfxScoreReached;

        private Texture2D _spriteSheetTexture;
        private Texture2D _fadeInTexture;

        private float _fadeInTexturePosX;
        
        private TRex _tRex;
        private InputController _inputController;

        private GroundManager _groundManager;

        private EntityManager _entityManager;

        private KeyboardState _previousKeyboardState;
        
        public GameState State { get; private set; }

        public TRexRunnerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _entityManager = new EntityManager();
            State = GameState.Initial;
            _fadeInTexturePosX = TRex.TREX_DEFAULT_SPRITE_WIDTH;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _sfxHit = Content.Load<SoundEffect>(ASSET_NAME_SFX_HIT);
            _sfxButtonPress = Content.Load<SoundEffect>(ASSET_NAME_SFX_BUTTON_PRESS);
            _sfxScoreReached = Content.Load<SoundEffect>(ASSET_NAME_SFX_SCORE_REACHED);
            _spriteSheetTexture = Content.Load<Texture2D>(ASSET_NAME_SPRITE_SHEET);
            
            _fadeInTexture = new Texture2D(GraphicsDevice, 1, 1);
            _fadeInTexture.SetData(new[] { Color.White });
            
            // test purpose only
            _tRex = new TRex(_spriteSheetTexture,
                new Vector2(TREX_START_POS_X,
                    TREX_START_POS_Y - TRex.TREX_DEFAULT_SPRITE_HEIGHT),
                _sfxButtonPress)
            {
                DrawOrder = 10
            };
            _tRex.JumpComplete += TRexOnJumpComplete;
            
            _inputController = new InputController(_tRex);
            
            _groundManager = new GroundManager(_spriteSheetTexture, _entityManager, _tRex);
            
            _entityManager.AddEntity(_tRex);
            _entityManager.AddEntity(_groundManager);
            
            // testing purposes
            _groundManager.Initialize();
        }

        private void TRexOnJumpComplete(object sender, EventArgs e)
        {
            if (State == GameState.Transition)
            {
                State = GameState.Playing;
                _tRex.Initialize();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();

            if (State == GameState.Playing)
            {
                _inputController.ProcessControls(gameTime);
            }
            else if (State == GameState.Transition)
            {
                _fadeInTexturePosX += (float)gameTime.ElapsedGameTime.TotalSeconds * FADE_IN_ANIMATION_SPEED;
            }
            else if (State == GameState.Initial)
            {
                var isStartKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
                var wasStartKeyPressed = _previousKeyboardState.IsKeyDown(Keys.Up) || _previousKeyboardState.IsKeyDown(Keys.Space);
                if (isStartKeyPressed && !wasStartKeyPressed)
                {
                    StartGame();
                }
            }
            
            _entityManager.Update(gameTime);

            _previousKeyboardState = keyboardState;

            // base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            
            _entityManager.Draw(_spriteBatch, gameTime);

            if (State == GameState.Initial || State == GameState.Transition)
            {
                _spriteBatch.Draw(_fadeInTexture, new Rectangle((int)Math.Round(_fadeInTexturePosX), 0, WINDOW_WIDTH, WINDOW_HEIGHT), Color.White);
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool StartGame()
        {
            if (State != GameState.Initial)
                return false;

            State = GameState.Transition;
            _tRex.BeginJump();

            return true;
        }
    }
}
