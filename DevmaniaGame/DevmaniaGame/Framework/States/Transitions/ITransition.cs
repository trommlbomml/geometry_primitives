
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States.Transitions
{
    interface ITransition
    {
        Texture2D Source { get; set; }
        Texture2D Target { get; set; }
        bool TransitionReady { get; set; }

        void Begin();
        void Update(float elapsedTime);
        void Render(SpriteBatch spriteBatch);
    }
}
