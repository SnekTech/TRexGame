using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities
{
    public class GroundManager : IGameEntity
    {
        private const float GROUND_TILE_POS_Y = 119;
        
        private const int SPRITE_WIDTH = 600;
        private const int SPRITE_HEIGHT = 14;

        private const int SPRITE_POS_X = 2;
        private const int SPRITE_POS_Y = 54;

        private Texture2D _spriteSheet;
        private readonly EntityManager _entityManager;

        private readonly List<GroundTile> _groundTiles;

        private Sprite _regularSprite;
        private Sprite _bumpySprite;

        private TRex _tRex;

        private Random _random;
        
        public int DrawOrder { get; set; }

        public GroundManager(Texture2D spriteSheet, EntityManager entityManager, TRex tRex)
        {
            _spriteSheet = spriteSheet;
            _groundTiles = new List<GroundTile>();
            _entityManager = entityManager;
            
            _regularSprite = new Sprite(spriteSheet, SPRITE_POS_X, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
            _bumpySprite = new Sprite(spriteSheet, SPRITE_POS_X + SPRITE_WIDTH, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            _tRex = tRex;
            _random = new Random();
        }
        
        public void Update(GameTime gameTime)
        {
            if (_groundTiles.Any())
            {
                var maxPosX = _groundTiles.Max(g => g.PositionX);
                if (maxPosX < 0)
                {
                    SpawnTile(maxPosX);
                }
            }
            
            var tilesToRemove = new List<GroundTile>();
            
            foreach (var groundTile in _groundTiles)
            {
                groundTile.PositionX -= _tRex.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (groundTile.PositionX < -SPRITE_WIDTH)
                {
                    // completely off the screen
                    _entityManager.RemoveEntity(groundTile);
                    tilesToRemove.Add(groundTile);
                }
            }

            foreach (var groundTile in tilesToRemove)
            {
                _groundTiles.Remove(groundTile);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public void Initialize()
        {
            _groundTiles.Clear();
            var groundTile = CreateRegularTile(0);
            _groundTiles.Add(groundTile);
            
            _entityManager.AddEntity(groundTile);
        }

        private GroundTile CreateRegularTile(float positionX)
        {
            var groundTile = new GroundTile(positionX, GROUND_TILE_POS_Y, _regularSprite);

            return groundTile;
        }
        
        private GroundTile CreateBumpyTile(float positionX)
        {
            var groundTile = new GroundTile(positionX, GROUND_TILE_POS_Y, _bumpySprite);

            return groundTile;
        }

        private void SpawnTile(float maxPosX)
        {
            var randomNumber = _random.NextDouble();

            var posX = maxPosX + SPRITE_WIDTH;

            var groundTile = randomNumber > 0.5 ? CreateBumpyTile(posX) : CreateRegularTile(posX);
            
            _entityManager.AddEntity(groundTile);
            _groundTiles.Add(groundTile);
        }
    }
}