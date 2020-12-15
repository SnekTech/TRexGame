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

            bool isJumpKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
            bool wasJumpKeyPressed = _previousKeyboardState.IsKeyDown(Keys.Up) ||
                                     _previousKeyboardState.IsKeyDown(Keys.Space);

            if (!wasJumpKeyPressed && isJumpKeyPressed)
            {
                if (_tRex.State != TRexState.Jumping)
                {
                    _tRex.BeginJump();
                }
            }
            else if (_tRex.State == TRexState.Jumping && !isJumpKeyPressed)
            {
                _tRex.CancelJump();
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (_tRex.State == TRexState.Jumping || _tRex.State == TRexState.Falling)
                {
                    _tRex.Drop();
                }
                else
                {
                    _tRex.Duck();
                }
            }
            else if (_tRex.State == TRexState.Ducking & !keyboardState.IsKeyDown(Keys.Down))
            {
                _tRex.GetUp();
            }

            _previousKeyboardState = keyboardState; // struct, so memberwise copy
        }
    }
}