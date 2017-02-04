
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.Drawing
{
    class AnimatedSprite
    {
        private readonly Texture2D _texture;
        private readonly float _timePerFrame;
        private readonly Rectangle[] _frames;
        private int _currentFrame;
        private float _currentTime;
        private readonly bool _looping;
        private SpriteEffects _spriteEffect;

        public float CurrentTime { get { return _currentTime; } }
        public int CurrentFrame { get { return _currentFrame; } }
        public bool Running { get; private set; }

        public AnimatedSprite(Texture2D texture, 
                              int frameWidth, 
                              int frameHeight,
                              int startX,
                              int startY, 
                              int frameCount, 
                              float animationTime, 
                              bool looping,
                              SpriteEffects effects)
        {
            _texture = texture;
            _timePerFrame = animationTime / frameCount;
            _looping = looping;
            _spriteEffect = effects;

            _frames = new Rectangle[frameCount];
            int currentX = startX * frameWidth;
            int currentY = startY * frameHeight;
            for(int i = 0; i < frameCount; i++)
            {
                _frames[i] = new Rectangle(currentX, currentY, frameWidth, frameHeight);
                currentX += frameWidth;
                if (currentX + frameWidth > _texture.Width)
                {
                    currentX = 0;
                    currentY -= frameHeight;
                }
            }
        }

        public void Play(int startFrame = 0, float elapsed = 0.0f)
        {
            if (!Running)
            {
                Running = true;
                _currentTime = elapsed;
                _currentFrame = startFrame;
                if (_currentFrame > _frames.Length)
                    _currentFrame = 0;
            }
        }

        public void Update(float elapsed)
        {
            if (!Running)
                return;

            _currentTime += elapsed;
            if (_currentTime >= _timePerFrame)
            {
                _currentTime -= _timePerFrame;
                
                if (++_currentFrame == _frames.Length)
                {
                    if (_looping)
                        _currentFrame = 0;
                    else
                    {
                        Running = false;
                        _currentFrame--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White, new Vector2(_frames[_currentFrame].Width, _frames[_currentFrame].Height) * 0.5f, Vector2.One, 0.0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 scale)
        {
            Draw(spriteBatch, position, Color.White, new Vector2(_frames[_currentFrame].Width, _frames[_currentFrame].Height) * 0.5f, scale, rotation);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 origin)
        {
            Draw(spriteBatch, position, Color.White, origin, Vector2.One, 0.0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            spriteBatch.Draw(_texture, position, _frames[_currentFrame], Color.White, rotation, origin, scale, _spriteEffect, 0);
        }

    }
}
