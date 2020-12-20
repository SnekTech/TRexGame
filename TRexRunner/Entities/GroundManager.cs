using System.Collections.Generic;
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
        
        public int DrawOrder { get; set; }

        public GroundManager(Texture2D spriteSheet, EntityManager entityManager, TRex tRex)
        {
            _spriteSheet = spriteSheet;
            _groundTiles = new List<GroundTile>();
            _entityManager = entityManager;
            
            _regularSprite = new Sprite(spriteSheet, SPRITE_POS_X, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
            _bumpySprite = new Sprite(spriteSheet, SPRITE_POS_X + SPRITE_WIDTH, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            _tRex = tRex;
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (var groundTile in _groundTiles)
            {
                groundTile.PositionX -= _tRex.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
    }
}