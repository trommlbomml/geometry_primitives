using System;
using DevmaniaGame.Framework.Input;
using DevmaniaGame.Framework.Sound;
using DevmaniaGame.Framework.States;
using DevmaniaGame.Framework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DevmaniaGame.Intro
{
    class IntroState : InitializableState
    {
        public static readonly Type Id = typeof(IntroState);

        private Texture2D _texture;

        protected override void OnEntered(object enterInformation)
        {
            SoundService.RegisterSong("backgroundmusic",StateManager.Content.Load<Song>("Music/air-ducts"));
            SoundService.PlaySong("backgroundmusic");
        }

        protected override void OnInitialize(object enterInformation)
        {
            _texture = StateManager.Content.Load<Texture2D>("textures/intro");
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            if (KeyboardEx.IsKeyDownOnce(Keys.Enter))
                return StateChangeInformation.StateChange(ManualState.Id, CardTransition.Id);

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }
    }
}
