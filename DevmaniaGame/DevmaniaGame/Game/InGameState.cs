
using System;
using System.Collections.Generic;
using DevmaniaGame.Framework.Drawing;
using DevmaniaGame.Framework.Sound;
using DevmaniaGame.Framework.States;
using DevmaniaGame.Framework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    class InGameState : InitializableState
    {
        public static readonly Type Id = typeof (InGameState);

        private const float MatchTime = 60.0f;

        private Level _level;
        private Player _player1;
        private Player _player2;
        private BalanceBar _balanceBar;

        private List<Monkey> _monkeys;
        private float _time;
        private EventManager _eventManager;
        private InGameCallOutRenderer _calloutRenderer;
        private Texture2D _finalCallout;

        private ParticleManager _particleManager;

        private SoundEffect _sfxFall;
        private SoundEffect _monkeySound;
        private SoundEffectInstance[] _monkeySoundInstances;
        private bool _sfxFallMonkeysPlayed;
        private bool _sfxFallPlayer1Played;
        private bool _sfxFallPlayer2Played;

        protected override void OnInitialize(object enterInformation)
        {
            DepthRenderer.Clear();
            ContentManager content = StateManager.Content;
            _particleManager= new ParticleManager(content);
            _level = new Level(content,_particleManager);

            _player1 = new Player(1, content) {Position = new Vector2(200, -300), AnimationTargetY = 380};
            _player2 = new Player(2, content) { Position = new Vector2(600, -300), AnimationTargetY = 420 };

            _monkeys = new List<Monkey>();
            _monkeys.Add(new Monkey(content, new Vector2(90, -300), true) {AnimationTargetY = 120});
            _monkeys.Add(new Monkey(content, new Vector2(700, -300), false) {AnimationTargetY = 140});
            _monkeys.Add(new Monkey(content, new Vector2(90, -300), true) {AnimationTargetY = 430});
            _monkeys.Add(new Monkey(content, new Vector2(700, -300), false) {AnimationTargetY = 430});

            _sfxFall = content.Load<SoundEffect>("Sound/fall");
            _sfxFallMonkeysPlayed = false;
            _sfxFallPlayer1Played = false;
            _sfxFallPlayer2Played = false;

            _balanceBar = new BalanceBar(content);
            _calloutRenderer = new InGameCallOutRenderer(content);

            _finalCallout = content.Load<Texture2D>("textures/finalcallouts");

            _monkeySound = content.Load<SoundEffect>("sound/monkey1");
            _monkeySoundInstances = new[]
            {
                _monkeySound.CreateInstance(),
                _monkeySound.CreateInstance(),
                _monkeySound.CreateInstance(),
                _monkeySound.CreateInstance()
            };
        }

        private float _winnerSplashAlpha;
        private void OnUpdateShowWinner(float elapsed, float progress)
        {
            if (progress < 0.2f)
                _winnerSplashAlpha = progress / 0.2f;
            else
                _winnerSplashAlpha = 1.0f;
        }

        private void OnDrawShowWinner(SpriteBatch spriteBatch)
        {
            int y;
            if (_player1.Score > _player2.Score)
                y = 256;
            else if (_player1.Score < _player2.Score)
                y = 0;
            else
                y = 512;

            spriteBatch.Draw(_finalCallout, new Vector2(400,300),new Rectangle(0, y, 1024, 256), 
                Color.White * _winnerSplashAlpha, 0.0f, new Vector2(512, 128),1.0f/(_winnerSplashAlpha>0?_winnerSplashAlpha:0.001f), SpriteEffects.None, 0 );
        }

        private void OnUpdateMonkeyYell(float elapsed, float progress)
        {
            if (progress >= 0.1f && _monkeySoundInstances[0].State == SoundState.Stopped)
                _monkeySoundInstances[0].Play();
            if (progress >= 0.3f && _monkeySoundInstances[1].State == SoundState.Stopped)
                _monkeySoundInstances[1].Play();
            if (progress >= 0.4f && _monkeySoundInstances[2].State == SoundState.Stopped)
                _monkeySoundInstances[2].Play();
            if (progress >= 0.7f && _monkeySoundInstances[3].State == SoundState.Stopped)
                _monkeySoundInstances[3].Play();
        }

        protected override void OnEntered(object enterInformation)
        {
            OnInitialize(enterInformation);

            _eventManager = new EventManager();
            _eventManager.AddEvent(1.0f, OnUpdateDropMonkeys);
            _eventManager.AddWait(0.0f);
            _eventManager.AddEvent(0.5f, OnUpdateDropPlayer1);
            _eventManager.AddWait(0.2f);
            _eventManager.AddEvent(0.5f, OnUpdateDropPlayer2);
            _eventManager.AddWait(0.5f);
            _eventManager.AddEvent(0.0f, (e, p) => _monkeys.ForEach(m => m.Start()));
            _eventManager.AddEvent(1.0f, OnUpdateBlendReady, OnRenderBlendReady);
            _eventManager.AddEvent(0.6f, OnUpdateBlendGo, OnRenderBlendGo);
            _eventManager.AddEvent(MatchTime, OnUpdateMatch);
            _eventManager.AddEvent(0.0f, (e, p) =>
                                             {
                                                 SoundService.StopCurrentSong();
                                                 _monkeys.ForEach(m => m.Stop());
                                             });
            _eventManager.AddWait(1.0f);
            _eventManager.AddEvent(3.0f, OnUpdateMonkeyYell);
            _eventManager.AddEvent(3.0f, OnUpdateShowWinner, OnDrawShowWinner);
            _eventManager.Start();
            _time = 0.0f;
        }

        private float _alphaReady;

        private void OnUpdateBlendReady(float elapsed, float progress)
        {
            if (progress < 0.5f)
                _alphaReady = progress/0.5f;
            else
                _alphaReady = 1.0f - (progress - 0.5f) / 0.5f;
        }

        private void OnRenderBlendReady(SpriteBatch spriteBatch)
        {
            _calloutRenderer.Draw(spriteBatch, InGameCallOutType.Ready, _alphaReady);
        }

        private void OnUpdateBlendGo(float elapsed, float progress)
        {
            if (progress < 0.1f)
                _alphaReady = 1;
            else
                _alphaReady = 1.0f - (progress - 0.1f) / 0.5f;
        }

        private void OnRenderBlendGo(SpriteBatch spriteBatch)
        {
            _calloutRenderer.Draw(spriteBatch, InGameCallOutType.Go, _alphaReady);
        }

        private void OnUpdateDropMonkeys(float elapsed, float progress)
        {
            foreach (Monkey monkey in _monkeys)
            {
                monkey.Position = new Vector2(monkey.Position.X, MathHelper.Lerp(-300, monkey.AnimationTargetY, progress * progress));
            }
            if ((1 - progress) * 1.5f < 0.3 && !_sfxFallMonkeysPlayed)
            {
                _sfxFall.Play();
                _sfxFallMonkeysPlayed = true;
            }
        }

        private void OnUpdateDropPlayer1(float elapsed, float progress)
        {
            if ((1 - progress) * 0.5f < 0.01 && !_sfxFallPlayer1Played)
            {
                _sfxFall.Play();
                _sfxFallPlayer1Played = true;
            }
            _player1.Position = new Vector2(_player1.Position.X, MathHelper.Lerp(-300, _player1.AnimationTargetY, progress));
        }

        private void OnUpdateDropPlayer2(float elapsed, float progress)
        {
            if ((1 - progress) * 0.5f < 0.01 && !_sfxFallPlayer2Played)
            {
                _sfxFall.Play();
                _sfxFallPlayer2Played = true;
            }

            _player2.Position = new Vector2(_player2.Position.X, MathHelper.Lerp(-300, _player2.AnimationTargetY, progress));
        }

        public override void OnLeave()
        {
        }

        private void OnUpdateMatch(float elapsedTime, float progress)
        {
            _time = progress*MatchTime;

            _level.Update(elapsedTime);
            _player1.Update(elapsedTime, _level, _monkeys);
            _player2.Update(elapsedTime, _level, _monkeys);
            _monkeys.ForEach(m => m.Update(elapsedTime, _player1, _player2));
            _particleManager.Update(elapsedTime);

            if (_player1.Bounds.Intersects(_player2.Bounds))
            {
                _player1.Bounce(_player2);
                _player2.Bounce(_player1);
            }
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _eventManager.Update(elapsedTime);

            if (!_eventManager.Running)
                return StateChangeInformation.StateChange(ResultGame.Id, CardTransition.Id);

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime, SpriteBatch spriteBatch)
        {
            _level.Draw(spriteBatch);
            _balanceBar.Render(spriteBatch, _player1, _player2, MatchTime - _time);
            DepthRenderer.Draw(spriteBatch);
            _monkeys.ForEach(m => m.DrawPopup(spriteBatch));
            _level.DrawFallingFood(spriteBatch);
            _eventManager.Render(spriteBatch);
            _player1.DrawCombo(spriteBatch);
            _player2.DrawCombo(spriteBatch);
            _particleManager.Draw(spriteBatch);
        }
    }
}
