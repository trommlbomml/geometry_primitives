using System;
using DevmaniaGame.Framework.Input;
using DevmaniaGame.Framework.States;
using DevmaniaGame.Framework.States.Transitions;
using DevmaniaGame.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.Intro
{
    class ManualState : InitializableState
    {
        public static readonly Type Id = typeof (ManualState);

        private Texture2D _texture;

        protected override void OnEntered(object enterInformation)
        {
        }

        protected override void OnInitialize(object enterInformation)
        {
            _texture = StateManager.Content.Load<Texture2D>("textures/instruction");
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            if (KeyboardEx.IsKeyDownOnce(Keys.Enter))
                return StateChangeInformation.StateChange(InGameState.Id, CardTransition.Id);

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }
    }
}
