
using System;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States
{
    class Event
    {
        public float Duration { get; private set; }
        public bool Retain { get; private set; }
        public Action<float,float> OnUpdate { get; private set; }
        public Action<SpriteBatch> OnRender { get; private set; }

        public Event(float duration, Action<float,float> updateAction, Action<SpriteBatch> renderAction, bool retain)
        {
            if (updateAction == null) throw new ArgumentNullException("updateAction");

            Duration = duration;
            Retain = retain;
            OnUpdate = updateAction;
            OnRender = renderAction;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (OnRender != null)
                OnRender(spriteBatch);
        }
    }
}
