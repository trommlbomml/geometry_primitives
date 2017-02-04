using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States.Transitions
{
    class BlendTransition : ITransition
    {
        public static readonly Type Id = typeof(BlendTransition);

        private const float BlendTime = 0.5f;
        private float _elapsedTime;

        public Texture2D Source { get; set; }
        public Texture2D Target { get; set; }

        public bool TransitionReady { get; set; }

        public BlendTransition()
        {
            TransitionReady = false;
        }

        public void Begin()
        {
            _elapsedTime = 0;
            TransitionReady = false;
        }

        public void Update(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            if (_elapsedTime >= BlendTime)
            {
                TransitionReady = true;
                _elapsedTime = BlendTime;
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            Texture2D toRender;
            float alpha;

            float delta = MathHelper.SmoothStep(0, 1, _elapsedTime/BlendTime);

            if (delta < 0.5f)
            {
                toRender = Source;
                alpha = 1.0f - delta * 2.0f;
            }
            else
            {
                toRender = Target;
                alpha = (delta - 0.5f) * 2.0f;
            }

            spriteBatch.Draw(toRender, Vector2.Zero, Color.White * alpha);
        }
    }
}
