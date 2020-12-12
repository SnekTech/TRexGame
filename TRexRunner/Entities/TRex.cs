using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities
{
    public class TRex : IGameEntity
    {
        private const int TREX_DEFAULT_SPRITE_POS_X = 848;
        private const int TREX_DEFAULT_SPRITE_POS_Y = 0;
        private const int TREX_DEFAULT_SPRITE_WIDTH = 44;
        private const int TREX_DEFAULT_SPRITE_HEIGHT = 52;
        
        public Sprite Sprite { get; set; }

        public TRexState State { get; private set; }

        public Vector2 Position { get; set; }

        public bool IsAlive { get; private set; }

        public float Speed { get; private set; }
        
        public int DrawOrder { get; set; }

        public TRex(Texture2D spriteSheet, Vector2 position)
        {
            Sprite = new Sprite(spriteSheet, TREX_DEFAULT_SPRITE_POS_X, TREX_DEFAULT_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            Position = position;
        }
     
        public void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}