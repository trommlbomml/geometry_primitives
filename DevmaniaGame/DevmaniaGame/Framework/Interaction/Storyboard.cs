
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DevmaniaGame.Framework.Interaction
{
    class StoryboardStep<T>
    {
        public T StartValue;
        public T TargetValue;
        public float Duration;
    }

    class FloatStoryboard : BaseStoryboard<float>
    {
        protected override float GetValue(float elapsedTime, StoryboardStep<float> step)
        {
            return MathHelper.Lerp(step.StartValue, step.TargetValue, elapsedTime / step.Duration);
        }
    }

    class Vector2Storyboard : BaseStoryboard<Vector2>
    {
        protected override Vector2 GetValue(float elapsedTime, StoryboardStep<Vector2> step)
        {
            return new Vector2(MathHelper.Lerp(step.StartValue.X, step.TargetValue.X, elapsedTime / step.Duration),
                               MathHelper.Lerp(step.StartValue.Y, step.TargetValue.Y, elapsedTime / step.Duration));
        }
    }

    abstract class BaseStoryboard<T>
    {
        private List<StoryboardStep<T>> _steps;
        private Queue<StoryboardStep<T>> _workItems;
        private StoryboardStep<T> _currentStep;
        private float _currentTime;
        private bool _looping;
        private T _currentValue;

        public bool Running { get; private set; }

        public BaseStoryboard()
        {
            _steps = new List<StoryboardStep<T>>();
        }

        public void AddStep(T start, T target, float duration)
        {
            _steps.Add(new StoryboardStep<T> { StartValue = start, Duration = duration, TargetValue = target });
        }

        public void Begin(bool looping)
        {
            _looping = looping;
            _workItems = new Queue<StoryboardStep<T>>(_steps);
            _currentStep = _workItems.Dequeue();
            _currentTime = 0;
            _currentValue = GetValue(_currentTime, _currentStep);
            Running = true;
        }

        protected abstract T GetValue(float elapsedTime, StoryboardStep<T> step);

        public void Update(float elapsedTime)
        {
            if (!Running)
                return;

            _currentTime += elapsedTime;
            if (_currentTime >= _currentStep.Duration)
            {
                _currentTime -= _currentStep.Duration;
                if (_workItems.Count == 0)
                {
                    if (_looping)
                        _workItems = new Queue<StoryboardStep<T>>(_steps);
                    else
                        Running = false;
                }
                else
                {
                    _currentStep = _workItems.Dequeue();
                }
            }

            _currentValue = GetValue(_currentTime, _currentStep);
        }

        public T CurrentValue { get { return _currentValue; } }
    }
}
