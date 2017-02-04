using System;
using DevmaniaGame.Framework.Input;
using DevmaniaGame.Framework.Sound;
using DevmaniaGame.Framework.States;
using DevmaniaGame.Framework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.Game
{
    class ResultGame : InitializableState
    {
        public static readonly Type Id = typeof (ResultGame); 

        private Texture2D _texture;
        private Texture2D _texture2;

        protected override void OnEntered(object enterInformation)
        {
            SoundService.PlaySong("backgroundmusic");
        }

        protected override void OnInitialize(object enterInformation)
        {
            _texture = StateManager.Content.Load<Texture2D>("textures/endgameselect");
            _texture2 = StateManager.Content.Load<Texture2D>("textures/anleitung_bg");
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            if (KeyboardEx.IsKeyDownOnce(Keys.Enter))
                return StateChangeInformation.StateChange(InGameState.Id, FlipTransition.Id);
            if (KeyboardEx.IsKeyDownOnce(Keys.Escape))
                return StateChangeInformation.QuitGameInformation(ThrowAwayTransition.Id);

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(new Color(29, 162, 131));
            spriteBatch.Draw(_texture2, Vector2.Zero, Color.White);
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }
    }
}
