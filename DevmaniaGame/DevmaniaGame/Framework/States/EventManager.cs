
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States
{

    class EventManager
    {
        private readonly List<Event> _events;
        private Queue<Event> _workList;
        private List<Event> _retaintedEvents; 
        private Event _currentEvent;
        private float _currentRunTime;

        public bool Running { get; private set; }

        public EventManager()
        {
            _events = new List<Event>();
            _retaintedEvents = new List<Event>();
        }

        /// <summary>
        /// Adds an Event to the Queue to work out.
        /// </summary>
        /// <param name="duration">Duration of Action in Seconds</param>
        /// <param name="updateAction">Action Called on Update. 
        /// First Parameter: Time elapsed since last call
        /// Second Parameter: Percentage Time elapsed since start of event.</param>
        /// <param name="renderAction">Action Called on Rendering</param>
        /// <param name="retain">will be Rendered and updated also duratin is over</param>
        public void AddEvent(float duration, Action<float,float> updateAction, Action<SpriteBatch> renderAction = null, bool retain = false)
        {
            _events.Add(new Event(duration, updateAction, renderAction, retain));
        }

        public void AddWait(float duration)
        {
            _events.Add(new Event(duration, (r,a) => { }, s => { }, false));
        }

        public void Start()
        {
            if (_events.Count == 0)
                return;

            Running = true;
            _workList = new Queue<Event>(_events);
            _retaintedEvents.Clear();
            _currentEvent = _workList.Dequeue();
            _currentRunTime = 0;
        }

        public void Stop()
        {
            Running = false;
            _workList = null;
            _currentEvent = null;
        }

        public void Update(float elapsed)
        {
            if (!Running)
                return;

            _currentRunTime += elapsed;
            _currentEvent.OnUpdate(elapsed, _currentRunTime / _currentEvent.Duration);

            _retaintedEvents.ForEach(e => e.OnUpdate(elapsed, 1.0f));

            if (_currentRunTime >= _currentEvent.Duration)
            {
                _currentRunTime -= _currentEvent.Duration;
                if (_currentEvent.Retain)
                    _retaintedEvents.Add(_currentEvent);
                _currentEvent = _workList.Count > 0 ? _workList.Dequeue() : null;
            }

            if (_currentEvent == null)
                Running = false;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!Running)
                return;
            _retaintedEvents.ForEach(e =>  e.Render(spriteBatch));
            _currentEvent.Render(spriteBatch);
        }
    }
}
