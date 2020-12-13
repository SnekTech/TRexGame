using System;

namespace TRexRunner.Graphics
{
    public class SpriteAnimationFrame
    {
        private Sprite _sprite;

        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "The sprite cannot be null.");
                }
            }
        }
        
        public float TimeStamp { get; }

        public SpriteAnimationFrame(Sprite sprite, float timeStamp)
        {
            _sprite = sprite;
            TimeStamp = timeStamp;
        }
    }
}