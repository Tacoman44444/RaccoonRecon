using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace AI.FiniteStateMachine
{
    public abstract class State
    {
        public readonly string name;
        public IStrategy strategy;
        public State(string name, IStrategy strategy)
        {
            this.name = name;
            this.strategy = strategy;
        }
        public void Initialize(IStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void Process()
        {
            strategy.Process();
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

    public class CombatState : State
    {

        public CombatState(string name, IStrategy strategy) : base(name, strategy)
        {

        }
    }
}