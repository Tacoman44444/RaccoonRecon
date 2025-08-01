using AI.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTrees 
{
    public interface ICommand
    {
        Node.Status Process();
        void Reset()
        {
            //default
        }
    }

    public class ActionCommand : ICommand
    {
        readonly Action doSomething;
        public ActionCommand(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Node.Status Process()
        {
            doSomething();
            return Node.Status.Success;
        }
    }

    public class Condition : ICommand
    {
        readonly Func<bool> condition;
        public Condition(Func<bool> condition)
        {
            this.condition = condition;
        }

        public Node.Status Process() => condition() ? Node.Status.Success : Node.Status.Failure;
    }

    public class PatrolCommand : ICommand
    {
        AIController controller;
        public PatrolCommand(AIController controller)
        {
            this.controller = controller;
        }

        public Node.Status Process()
        {
            return controller.PatrolAction();
        }

        public void Reset()
        {
            controller.ResetPatrol();
        }
    }

    public class AlertCommand : ICommand
    {
        AIController controller;
        Transform alertCue;
        public AlertCommand(AIController controller, Transform alertCue)
        {
            this.controller = controller;
            this.alertCue = alertCue;
        }

        public Node.Status Process()
        {
            return controller.AlertAction(alertCue);
        }

        public void Reset()
        {
            controller.ResetAlert();
        }
    }

    public class SearchCommand : ICommand
    {
        AIController controller;
        public SearchCommand(AIController controller)
        {
            this.controller = controller;
        }

        public Node.Status Process()
        {
            return controller.SearchAction();
        }

        public void Reset()
        {
            controller.ResetSearch();
        }

    }

    public class CombatCommand : ICommand
    {
        AIController controller;
        Transform player;
        public CombatCommand(AIController controller)
        {
            this.controller = controller;
        }

        public Node.Status Process()
        {
            return controller.CombatAction(player);
        }

        public void Reset()
        {
            controller.ResetCombat();
        }
    }

}
