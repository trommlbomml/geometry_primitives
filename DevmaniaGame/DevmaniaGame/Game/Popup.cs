
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    enum PopupType
    {
        Wrong=0,
        Combo=1,
        Right=2
    }
    class Popup
    {
        private const float PopupDuration = 0.8f;
        private const float DriftSpeed = 50;
        public Vector2 StartPosition;
        private Vector2 _position;
        private Texture2D _image;
        private float _timer;
        private Rectangle _rect;
        public Popup(ContentManager content)
        {
            _image = content.Load<Texture2D>("Textures/minilines");
        }
        public void Activate(PopupType type)
        {
            _position = StartPosition;
            _rect=new Rectangle(0,(3-(int)type)*32,128,32);
            _timer = PopupDuration;
        }
        public void Update(float elapsedTime)
        {
            if (_timer < 0)
                return;

            _timer -= elapsedTime;
            _position.Y -= DriftSpeed*elapsedTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_timer < 0)
                return;
            spriteBatch.Draw(_image,_position,_rect,Color.White*MathHelper.SmoothStep(1,0,_timer/PopupDuration),0,new Vector2(64,16),1,SpriteEffects.None,0);
        }
    }
}
