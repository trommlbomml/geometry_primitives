
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.Framework.Input
{
    class KeyboardEx
    {
        private static KeyboardState _lastState;
        private static KeyboardState _currentState;
        private static Keys[] _pressedKeys;

        static KeyboardEx()
        {
            _currentState = Keyboard.GetState();
        }

        public static bool IsKeyDownOnce(Keys key)
        {
            return _currentState.IsKeyDown(key) && _lastState.IsKeyUp(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return _currentState.IsKeyUp(key);
        }

        public static Keys[] GetPressedKeys()
        {
            return _pressedKeys;
        }

        public static void Update()
        {
            _lastState = _currentState;
            _currentState = Keyboard.GetState();
            _pressedKeys = _currentState.GetPressedKeys();
        }
    }
}
