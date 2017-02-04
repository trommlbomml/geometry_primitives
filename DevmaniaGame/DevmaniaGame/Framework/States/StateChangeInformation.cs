
using System;

namespace DevmaniaGame.Framework.States
{
    class StateChangeInformation
    {
        public Type TargetState { get; private set; }
        public Type Transition { get; private set; }
        public bool QuitGame { get; private set; }
        public object EnterInformation { get; private set; }

        public static StateChangeInformation Empty = new StateChangeInformation();

        public StateChangeInformation()
        {
            TargetState = null;
            Transition = null;
            QuitGame = false;
            EnterInformation = null;
        }

        public static StateChangeInformation QuitGameInformation(Type transition)
        {
            return new StateChangeInformation
            {
                QuitGame = true,
                Transition = transition,
            };
        }

        public static StateChangeInformation StateChange(Type targetState, Type transition, object enterInformation)
        {
            return new StateChangeInformation
            {
                TargetState = targetState,
                Transition = transition,
                EnterInformation = enterInformation,
            };
        }

        public static StateChangeInformation StateChange(Type targetState, Type transition)
        {
            return StateChange(targetState, transition, null);
        }
    }
}
