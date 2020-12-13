using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TRexRunner.Entities;

namespace TRexRunner.System
{
    public class InputController
    {
        private TRex _tRex;

        private KeyboardState _previousKeyboardState;

        public InputController(TRex tRex)
        {
            _tRex = tRex;
            _previousKeyboardState = Keyboard.GetState(); // prevent it to be null?
        }

        public void ProcessControls(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (!_previousKeyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyDown(Keys.Up))
            {
                if (_tRex.State != TRexState.Jumping)
                {
                    _tRex.BeginJump();
                }
                else
                {
                    _tRex.ContinueJump();
                }
            }

            _previousKeyboardState = keyboardState; // struct, so memberwise copy
        }
    }
}