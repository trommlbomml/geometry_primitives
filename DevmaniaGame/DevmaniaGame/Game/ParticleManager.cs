
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    class ParticleManager
    {
        private readonly List<Particle> _particles;
        private readonly ContentManager _content;

        public ParticleManager(ContentManager content)
        {
            _content = content;
            _particles=new List<Particle>();
        }

        public void AddParticle(ParticleType type,Vector2 position)
        {
            _particles.Add(new Particle(_content,type,position));
        }

        public void Update(float elapsedTime)
        {
            _particles.ForEach(p => p.Update(elapsedTime));
            _particles.RemoveAll(p => !p.Alive);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in _particles)
            {
                particle.Draw(spriteBatch);
            }
        }
    }
}
