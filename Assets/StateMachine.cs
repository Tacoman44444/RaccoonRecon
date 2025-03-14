using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FiniteStateMachine
{
    public class StateMachine
    {

        private State currentState;
        public Dictionary<string, State> states;

        public StateMachine(State startState, Dictionary<string, State> allStates)
        {
            currentState = startState;
            states = allStates;
        }
        public void Process()
        {
            currentState.Process();
        }
        public void ChangeState(string eventName)
        {
            currentState = currentState.ChangeState(eventName) ?? currentState;
        }
    }
}