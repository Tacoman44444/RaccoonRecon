using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace AI.FiniteStateMachine
{
    public class State
    {
        public readonly string name;
        public readonly IStrategy strategy;
        public readonly Dictionary<String, State> transitions;

        public State(string name, IStrategy strategy)
        {
            this.name = name;
            this.strategy = strategy;
            transitions = new Dictionary<String, State>();
        }

        public void Process()
        {
            strategy.Process();
        }
        public void AddTransition(string eventName, State state)
        {
            transitions.Add(eventName, state);
        }
        public State ChangeState(string eventName)
        {
            if (transitions.TryGetValue(eventName, out State state)) return state;
            return null;
        }
    }

    public class PatrolState : State
    {
        public PatrolState(string name, IStrategy strategy) : base(name, strategy)
        {
            
        }
    }

    public class ArousedState : State
    {

        public ArousedState(string name, IStrategy strategy) : base(name, strategy)
        {

        }
    }
}