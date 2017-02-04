
using DevmaniaGame.Framework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    class FallingFood
    {
        private const float FallHeight = 200;

        public Food MyFood;
        private float _timer;
        private readonly float _fallSpeed;
        private readonly Vector2 _destinationPosition;

        private const float Delay = 0.0f;
        private float _delayTimer;

        private readonly Texture2D _shadow;

        public FallingFood(Food food, float speed, Vector2 destinationPosition)
        {
            MyFood = food;
            DepthRenderer.UnRegister(MyFood);
            _timer = FallHeight/speed;
            _fallSpeed = speed;
            _destinationPosition = destinationPosition;
            MyFood.Position = _destinationPosition + new Vector2(0, -FallHeight);
            _delayTimer = Delay;
            _shadow = MyFood.Shadow;
        }

        public bool Update(float elapsedTime)
        {
            if (_delayTimer > 0)
            {
                _delayTimer -= elapsedTime;
                return true;
            }
            _timer -= elapsedTime;
            if (_timer < 0)
                _timer = 0;
            MyFood.Position = _destinationPosition + new Vector2(0, -(_fallSpeed*_timer));
            return _timer != 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            MyFood.Draw(spriteBatch);
        }

        public void DrawShadow(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_shadow, 
                             _destinationPosition + new Vector2(0, 20), 
                             null, 
                             Color.White, 
                             0, 
                             new Vector2(64, 108), Food.GetScaleFromFoodSize(MyFood.FoodSize)* 1.2f, SpriteEffects.None, 0);
        }
    }
}
