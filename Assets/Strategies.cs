using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
}