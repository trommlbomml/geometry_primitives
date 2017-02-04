
using DevmaniaGame.Framework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    enum ParticleType
    {
        PoofMini=0,
        PoofMedium=1,
        PoofLarge=2,
        Blinky=3,
        DustL=4,
        DustR=5
    }
    class Particle
    {
        private AnimatedSprite _sprite;
        private Vector2 _position;
        public Particle(ContentManager content,ParticleType type, Vector2 position)
        {
            switch(type)
            {
                case ParticleType.PoofMini:
                    _sprite=new AnimatedSprite(content.Load<Texture2D>("Textures/Poof_mini"),64,64,0,4,12,0.5f,false,SpriteEffects.None);
                    break;
                case ParticleType.PoofMedium:
                    _sprite = new AnimatedSprite(content.Load<Texture2D>("Textures/Poof_medium"), 128, 128, 0, 4, 12, 0.5f, false, SpriteEffects.None);
                    break;
                case ParticleType.PoofLarge:
                    _sprite = new AnimatedSprite(content.Load<Texture2D>("Textures/Poof_large"), 256, 256, 0, 4, 12, 0.5f, false, SpriteEffects.None);
                    break;
                case ParticleType.Blinky:
                    _sprite = new AnimatedSprite(content.Load<Texture2D>("Textures/sparkle"), 16, 16, 0, 4, 5, 0.7f, false, SpriteEffects.None);
                    break;
                case ParticleType.DustL:
                    _sprite = new AnimatedSprite(content.Load<Texture2D>("Textures/dust"), 64, 64, 0, 1, 4, 0.5f, false, SpriteEffects.None);
                    break;
                case ParticleType.DustR:
                    _sprite = new AnimatedSprite(content.Load<Texture2D>("Textures/dust"), 64, 64, 0, 1, 4, 0.5f, false, SpriteEffects.FlipHorizontally);
                    break;
            }
            _sprite.Play();
            _position = position;
        }

        public void Update(float elapsedTime)
        {
            _sprite.Update(elapsedTime);
        }

        public bool Alive {get { return _sprite.Running; }}

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch,_position);
        }

    }
}
