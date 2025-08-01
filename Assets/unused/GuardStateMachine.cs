using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FiniteStateMachine
{
    public class GuardStateMachine
    {

        private State currentState;
        private readonly Dictionary<(string, State), State> transitionTable;

        public GuardStateMachine(State startState, Dictionary<(string, State), State> transitionTable)
        {
            currentState = startState;
            this.transitionTable = transitionTable;
        }
        public void Process()
        {
            currentState.Process();
        }
        public void ChangeState(string eventName, IGuardStrategies strategy)
        {

            if (transitionTable.TryGetValue((eventName, currentState), out State state))
            {
                Debug.Log("Changing state");
                state.Initialize(strategy);
                currentState = state;
            }
        }
    }
}