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
        public IGuardStrategies strategy;
        public State(string name, IGuardStrategies strategy)
        {
            this.name = name;
            this.strategy = strategy;
        }
        public void Initialize(IGuardStrategies strategy)
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
        public PatrolState(string name, IGuardStrategies strategy) : base(name, strategy)
        {
            
        }
    }

    public class ArousedState : State
    {

        public ArousedState(string name, IGuardStrategies strategy) : base(name, strategy)
        {

        }
    }

    public class CombatState : State
    {

        public CombatState(string name, IGuardStrategies strategy) : base(name, strategy)
        {

        }
    }

    public class SleepState : State
    {

        public SleepState(string name, IGuardStrategies strategy) : base(name, strategy)
        {

        }
    }
}