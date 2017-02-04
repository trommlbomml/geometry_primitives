using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States
{
    interface IState
    {
        StateManager StateManager { get; set; }
        void OnEnter(object enterInformation);
        void OnLeave();
        StateChangeInformation OnUpdate(float elapsedTime);
        void OnDraw(float elapsedTime, SpriteBatch spriteBatch);
    }
}
