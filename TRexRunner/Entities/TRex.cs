﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities
{
    public class TRex : IGameEntity
    {
        private const float START_SPEED = 240f;
        
        private const float RUN_ANIMATION_FRAME_LENGTH = 0.1f;
        
        private const float MIN_JUMP_HEIGHT = 40f;
        
        private const float GRAVITY = 1600f;
        private const float JUMP_START_VELOCITY = -480f;

        private const float CANCEL_JUMP_VELOCITY_DECREASE_FACTOR = 0.1f; 
        
        private const int TREX_IDLE_BACKGROUND_SPRITE_POS_X = 40;
        private const int TREX_IDLE_BACKGROUND_SPRITE_POS_Y = 0;
        
        public const int TREX_DEFAULT_SPRITE_POS_X = 848;
        public const int TREX_DEFAULT_SPRITE_POS_Y = 0;
        public const int TREX_DEFAULT_SPRITE_WIDTH = 44;
        public const int TREX_DEFAULT_SPRITE_HEIGHT = 52;

        private const float BLINK_ANIMATION_RANDOM_MIN = 2f;
        private const float BLINK_ANIMATION_RANDOM_MAX = 10f;
        private const float BLINK_ANIMATION_EYE_CLOSE_TIME = 0.5f;

        private const int TREX_RUNNING_SPRITE_ONE_POS_X = TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_WIDTH * 2;
        private const int TREX_RUNNING_SPRITE_ONE_POS_Y = 0;

        private const int TREX_DUCKING_SPRITE_WIDTH = 59;
        
        private const int TREX_DUCKING_SPRITE_ONE_POS_X = TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_WIDTH * 6;
        private const int TREX_DUCKING_SPRITE_ONE_POS_Y = 0;

        private const float DROP_VELOCITY = 600f;

        private Sprite _idleBackgroundSprite;

        private Sprite _idleSprite;
        private Sprite _idleBlinkSprite;

        private SoundEffect _jumpSound;
        
        private SpriteAnimation _blinkAnimation;
        private SpriteAnimation _runAnimation;
        private SpriteAnimation _duckAnimation;

        private Random _random;

        private float _verticalVelocity;
        private float _startPosY;
        private float _dropVelocity;

        public event EventHandler JumpComplete;

        public TRexState State { get; private set; }

        public Vector2 Position { get; set; }

        public bool IsAlive { get; private set; }

        public float Speed { get; private set; } = 0;
        
        public int DrawOrder { get; set; }

        public TRex(Texture2D spriteSheet, Vector2 position, SoundEffect jumpSound)
        {
            Position = position;
            _idleBackgroundSprite = new Sprite(spriteSheet,
                TREX_IDLE_BACKGROUND_SPRITE_POS_X,
                TREX_IDLE_BACKGROUND_SPRITE_POS_Y,
                TREX_DEFAULT_SPRITE_WIDTH,
                TREX_DEFAULT_SPRITE_HEIGHT);
            
            State = TRexState.Idle;
            _jumpSound = jumpSound;
            
            _random = new Random();
            
            _idleSprite = new Sprite(spriteSheet,
                TREX_DEFAULT_SPRITE_POS_X,
                TREX_DEFAULT_SPRITE_POS_Y,
                TREX_DEFAULT_SPRITE_WIDTH,
                TREX_DEFAULT_SPRITE_HEIGHT);
            _idleBlinkSprite = new Sprite(spriteSheet,
                TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_WIDTH,
                TREX_DEFAULT_SPRITE_POS_Y,
                TREX_DEFAULT_SPRITE_WIDTH,
                TREX_DEFAULT_SPRITE_HEIGHT);
            
            _blinkAnimation = new SpriteAnimation();
            CreateBlinkAnimation();
            _blinkAnimation.Play();

            _startPosY = position.Y;
            
            _runAnimation = new SpriteAnimation();
            var runningSprite1 = new Sprite(spriteSheet, TREX_RUNNING_SPRITE_ONE_POS_X, TREX_RUNNING_SPRITE_ONE_POS_Y,
                TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            var runningSprite2 = new Sprite(spriteSheet, TREX_RUNNING_SPRITE_ONE_POS_X + TREX_DEFAULT_SPRITE_WIDTH, TREX_RUNNING_SPRITE_ONE_POS_Y,
                TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            _runAnimation.AddFrame(runningSprite1, 0);
            _runAnimation.AddFrame(runningSprite2, RUN_ANIMATION_FRAME_LENGTH);
            _runAnimation.AddFrame(runningSprite1, RUN_ANIMATION_FRAME_LENGTH * 2);
            _runAnimation.Play();
            
            _duckAnimation = new SpriteAnimation();
            var duckingSprite1 = new Sprite(spriteSheet, TREX_DUCKING_SPRITE_ONE_POS_X, TREX_DUCKING_SPRITE_ONE_POS_Y,
                TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            var duckingSprite2 = new Sprite(spriteSheet, TREX_DUCKING_SPRITE_ONE_POS_X + TREX_DUCKING_SPRITE_WIDTH, TREX_DUCKING_SPRITE_ONE_POS_Y,
                TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            _duckAnimation.AddFrame(duckingSprite1, 0);
            _duckAnimation.AddFrame(duckingSprite2, RUN_ANIMATION_FRAME_LENGTH);
            _duckAnimation.AddFrame(duckingSprite1, RUN_ANIMATION_FRAME_LENGTH * 2);
            _duckAnimation.Play();
        }
     
        public void Update(GameTime gameTime)
        {
            if (State == TRexState.Idle)
            {
                if (!_blinkAnimation.IsPlaying)
                {
                    CreateBlinkAnimation();
                    _blinkAnimation.Play();
                }
                _blinkAnimation.Update(gameTime);
            }
            else if (State == TRexState.Jumping || State == TRexState.Falling)
            {
                Position = new Vector2(Position.X, Position.Y + (_verticalVelocity + _dropVelocity) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                _verticalVelocity += GRAVITY * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_verticalVelocity >= 0)
                {
                    State = TRexState.Falling;
                }
                
                if (Position.Y >= _startPosY)
                {
                    Position = new Vector2(Position.X, _startPosY);
                    _verticalVelocity = 0;
                    State = TRexState.Running;
                    
                    OnJumpComplete();
                }
            }
            else if (State == TRexState.Running)
            {
                _runAnimation.Update(gameTime);
            }
            else if (State == TRexState.Ducking)
            {
                _duckAnimation.Update(gameTime);
            }

            _dropVelocity = 0;
        }

        public void Initialize()
        {
            Speed = START_SPEED;
            State = TRexState.Running;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State == TRexState.Idle)
            {
                _idleBackgroundSprite.Draw(spriteBatch, Position);
                _blinkAnimation.Draw(spriteBatch, Position);
            }
            else if (State == TRexState.Jumping || State == TRexState.Falling)
            {
                _idleSprite.Draw(spriteBatch, Position);
            }
            else if (State == TRexState.Running)
            {
                _runAnimation.Draw(spriteBatch, Position);
            }
            else if (State == TRexState.Ducking)
            {
                _duckAnimation.Draw(spriteBatch, Position);
            }
        }

        private void CreateBlinkAnimation()
        {
            _blinkAnimation.Clear();
            _blinkAnimation.ShouldLoop = false;

            var blinkTimeStamp = BLINK_ANIMATION_RANDOM_MIN + _random.NextDouble() * (BLINK_ANIMATION_RANDOM_MAX - BLINK_ANIMATION_RANDOM_MIN);
            
            _blinkAnimation.AddFrame(_idleSprite, 0);
            _blinkAnimation.AddFrame(_idleBlinkSprite, (float)blinkTimeStamp);
            _blinkAnimation.AddFrame(_idleSprite, (float)blinkTimeStamp + BLINK_ANIMATION_EYE_CLOSE_TIME); // dummy end frame
        }

        public bool BeginJump()
        {
            if (State == TRexState.Jumping || State == TRexState.Falling)
            {
                return false;
            }

            _jumpSound.Play();
            State = TRexState.Jumping;
            _verticalVelocity = JUMP_START_VELOCITY;
            
            return true;
        }

        public bool CancelJump()
        {
            if (State != TRexState.Jumping || (_startPosY - Position.Y) < MIN_JUMP_HEIGHT)
            {
                return false;
            }

            _verticalVelocity *= CANCEL_JUMP_VELOCITY_DECREASE_FACTOR;
            return true;
        }

        public bool Duck()
        {
            if (State == TRexState.Jumping || State == TRexState.Falling)
            {
                return false;
            }

            State = TRexState.Ducking;

            return true;
        }

        public bool GetUp()
        {
            if (State != TRexState.Ducking)
            {
                return false;
            }

            State = TRexState.Running;
            return true;
        }

        public bool Drop()
        {
            if (State != TRexState.Falling && State != TRexState.Jumping)
            {
                return false;
            }

            State = TRexState.Falling;

            _dropVelocity = DROP_VELOCITY;

            return true;
        }

        protected virtual void OnJumpComplete()
        {
            var handler = JumpComplete;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}