using AI.Pathfinding;
using BlackboardSystem;
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
            Node.Status status = controller.PatrolAction();
            return status;
        }

        public void Reset()
        {
            controller.ResetPatrol();
        }
    }

    public class AlertCommand : ICommand
    {
        AIController controller;
        Blackboard blackboard;
        BlackboardKey cueKey;
        BlackboardKey detected;
        public AlertCommand(AIController controller, Blackboard blackboard)
        {
            this.controller = controller;
            this.blackboard = blackboard;
            cueKey = blackboard.GetOrRegisterKey("AlertCueKey");
            detected = blackboard.GetOrRegisterKey("IsDetected");
        }

        public Node.Status Process()
        {
            if (blackboard.TryGetValue<bool>(detected, out bool val))
            {
                return Node.Status.Failure;
            }
            if (blackboard.TryGetValue<Vector2>(cueKey, out Vector2 cue))
            {
                Node.Status alertStatus = controller.AlertAction(cue);
                return alertStatus;
            }
            Debug.Log("ERROR::AlertCommand Failed, could not get value from blackboard");
            return Node.Status.Failure;
        }

        public void Reset()
        {
            controller.ResetAlert();
            BlackboardKey servedKey = blackboard.GetOrRegisterKey("AlertServed");
            blackboard.SetValue(servedKey, true);
        }
    }

    public class SearchCommand : ICommand
    {
        AIController controller;
        Blackboard blackboard;
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

    public class DetectPlayerCommandOrc : ICommand
    {
        private AIController controller;
        private Func<bool> condition;
        private float detectTimer;
        public DetectPlayerCommandOrc(AIController controller, Func<bool> condition)
        {
            this.controller = controller;
            this.condition = condition;
        }

        public Node.Status Process()
        {
            detectTimer += Time.deltaTime;
            if (detectTimer > controller.detectPlayerInterval)
            {
                return Node.Status.Success;
            }

            if (!condition())
            {
                return Node.Status.Failure;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            detectTimer = 0.0f;
        }
    }

    public class CombatCommand : ICommand
    {
        AIController controller;
        Transform player;
        public CombatCommand(AIController controller, Transform player)
        {
            this.controller = controller;
            this.player = player;
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

    public class DrunkCommand : ICommand
    {
        AIController controller;
        float drunkTimer = 0.0f;
        float drunkDuration;

        public DrunkCommand(AIController controller, float drunkDuration)
        {
            this.controller = controller;
            this.drunkDuration = drunkDuration;
        }

        public Node.Status Process()
        {
            if (drunkTimer > drunkDuration)
            {
                return Node.Status.Success;
            }
            drunkTimer += Time.deltaTime;
            return controller.RandomWalk(0.0f, 2.0f);
        }

        public void Reset()
        {
            controller.ResetRandomWalk();
            drunkTimer = 0.0f;
        }
    }
}
