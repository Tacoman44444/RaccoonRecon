using AI.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

namespace AI
{
    public interface IStrategy
    {
        void Process();
    }

    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;
        readonly List<Transform> patrolPoints;
        readonly float patrolSpeed;
        private AStar pathfinder;
        private int currentIndex = 0;
        public PatrolStrategy(Transform entity, List <Transform> patrolPoints, float patrolSpeed) 
        { 
            this.entity = entity;
            this.patrolPoints = patrolPoints;
            this.patrolSpeed = patrolSpeed;
        }

        public void Process()
        {
            if (patrolPoints.Count == 0) return;

            if (currentIndex >= patrolPoints.Count)
            {
                currentIndex = 0;
            }

            Transform target = patrolPoints[currentIndex];
            MoveTowards(target);

            if (Vector3.Distance(entity.transform.position, target.transform.position) < 0.5f)
            {
                currentIndex++;
            }
        }

        private void MoveTowards(Transform target)
        {
            Vector3 direction = (target.transform.position - entity.transform.position).normalized;
            entity.transform.position += direction * patrolSpeed * Time.deltaTime;
        }

    }

    public class ArousedStrategy : IStrategy
    {
        readonly Transform entity;
        readonly Transform alertCue;
        private AStar pathfinder;
        private List<Node> path;
        private int pathIndex = 0;
        readonly float searchSpeed = 2.0f;
        private float arousedTimer = 10.0f;

        public ArousedStrategy(Transform entity, Transform alertCue, AStar pathfinder, float searchSpeed)
        {
            this.entity = entity;
            this.alertCue = alertCue;
            this.pathfinder = pathfinder;
            this.searchSpeed = searchSpeed;
            Initialize();
        }

        private void Initialize()
        {
            path = pathfinder.FindPath(entity.transform.position, alertCue.transform.position);
            pathIndex = 0;
        } 

        public void Process()
        {
            if (path != null && pathIndex < path.Count)
            {
                Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
                entity.transform.position = Vector2.MoveTowards(entity.transform.position, targetPos, 2f * Time.deltaTime);

                if (Vector2.Distance(entity.transform.position, targetPos) < 0.3f)
                    pathIndex++;
            }
        }
    }
}