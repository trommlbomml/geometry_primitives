using System;
using System.Collections.Generic;
using DevmaniaGame.Framework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States
{
    class StateManager
    {
        private readonly Dictionary<Type, IState> _availableStates;
        private readonly Dictionary<Type, ITransition> _availableTransitions;
        private bool _afterTransitionQuit;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _sourceRenderTarget;
        private readonly RenderTarget2D _targetRenderTarget;
        private bool _transitionInProgress;
        private bool _transitionStarted;
        private ITransition _currentTransition;
        private IState _oldStateForSourceTransition;
        private IState _currentState;

        public ContentManager Content { get; set; }

        public StateManager(GraphicsDevice device)
        {
            _graphicsDevice = device;
            _availableStates = new Dictionary<Type, IState>();
            _availableTransitions = new Dictionary<Type, ITransition>();
            _sourceRenderTarget = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height);
            _targetRenderTarget = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height);
        }

        public void RegisterTransition(ITransition transition)
        {
            Type t = transition.GetType();

            if (_availableTransitions.ContainsKey(t))
                throw new InvalidOperationException("Transition Type alread exists");
            if (transition == null)
                throw new ArgumentNullException("transition");

            _availableTransitions.Add(t, transition);
        }

        public void RegisterState(IState state)
        {
            Type t = state.GetType();

            if (_availableStates.ContainsKey(state.GetType()))
                throw new InvalidOperationException("State Type already exists");
            if (state == null)
                throw new ArgumentNullException("state");

            _availableStates.Add(t, state);
            state.StateManager = this;
        }

        public void SetCurrentState(Type state, object enterInformation)
        {
            IState newState;
            if (!_availableStates.TryGetValue(state, out newState))
                throw new InvalidOperationException("State not registered");

            if (_currentState != null)
                _currentState.OnLeave();

            _currentState = newState;
            newState.OnEnter(enterInformation);
        }

        public void QuitGame(Type transition)
        {
            _afterTransitionQuit = true;
            _transitionInProgress = true;
            _transitionStarted = false;
            _currentTransition = _availableTransitions[transition];
            _currentTransition.Source = null;
            _currentTransition.Target = null;
            _oldStateForSourceTransition = _currentState;

            _currentState.OnLeave();
            _currentState = null;
        }

        public void ChangeToState(Type state, Type transition, object enterInformation)
        {
            IState newState;
            if (!_availableStates.TryGetValue(state, out newState))
                throw new InvalidOperationException("State not registered");

            _afterTransitionQuit = false;
            _transitionInProgress = true;
            _transitionStarted = false;
            _currentTransition = _availableTransitions[transition];
            _currentTransition.Source = null;
            _currentTransition.Target = null;
            _oldStateForSourceTransition = _currentState;

            _currentState.OnLeave();

            _currentState = newState;
            newState.OnEnter(enterInformation);
        }

        public bool Update(GameTime time)
        {
            float elapsed = time.ElapsedGameTime.Milliseconds*0.001f;

            if (_transitionInProgress)
            {
                if(!_transitionStarted)
                {
                    _currentTransition.Begin();
                    _transitionStarted = true;
                }

                _currentTransition.Update(elapsed);
                if (_currentTransition.TransitionReady)
                {
                    _transitionInProgress = false;
                    return !_afterTransitionQuit;
                }
            }
            else
            {
                StateChangeInformation stateChangeInformation = _currentState.OnUpdate(elapsed);
                if (stateChangeInformation != StateChangeInformation.Empty)
                {
                    if (stateChangeInformation.QuitGame)
                    {
                        QuitGame(stateChangeInformation.Transition);
                    }
                    else
                    {
                        ChangeToState(
                        stateChangeInformation.TargetState,
                        stateChangeInformation.Transition,
                        stateChangeInformation.EnterInformation);    
                    }

                    return true;
                }
            }

            return true;
        }

        private static void DrawState(float elapsedTime, SpriteBatch spriteBatch, IState toRender)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
            toRender.OnDraw(elapsedTime, spriteBatch);
            spriteBatch.End();
        }

        private void PreRenderSourceAndTargetState(float elapsedTime, SpriteBatch spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(_sourceRenderTarget);
            _graphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            DrawState(elapsedTime, spriteBatch, _oldStateForSourceTransition);
            
            if (_currentState != null)
            {
                _graphicsDevice.SetRenderTarget(_targetRenderTarget);
                _graphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
                DrawState(elapsedTime, spriteBatch, _currentState);    
            }

            _graphicsDevice.SetRenderTarget(null);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds*0.001f;

            if (_transitionInProgress)
            {
                if (_currentTransition.Source == null && _currentTransition.Target == null)
                {
                    PreRenderSourceAndTargetState(elapsedTime, spriteBatch);
                    _currentTransition.Source = _sourceRenderTarget;
                    _currentTransition.Target = _targetRenderTarget;
                }

                _graphicsDevice.Clear(ClearOptions.Target|ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
                _currentTransition.Render(spriteBatch);
                spriteBatch.End();
            }
            else if (_currentState != null)
            {
                DrawState(elapsedTime, spriteBatch, _currentState);
            }
        }
    }
}
