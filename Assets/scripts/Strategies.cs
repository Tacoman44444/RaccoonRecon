using AI.Pathfinding;
using HelperFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

namespace AI
{
    public interface IGuardStrategies
    {
        void Process();
    }

    public class PatrolStrategy : IGuardStrategies
    {
        readonly Transform entity;
        readonly List<Transform> patrolPoints;
        readonly float patrolSpeed;
        private AStar pathfinder;
        private List<Tile> path;
        private int pathIndex = 0;
        private int currentIndex = 0;
        private float timer = 0.0f;
        private float lookAroundTimer = 0.8f;
        public PatrolStrategy(Transform entity, List <Transform> patrolPoints, AStar pathfinder, float patrolSpeed) 
        { 
            this.entity = entity;
            this.patrolPoints = patrolPoints;
            this.patrolSpeed = patrolSpeed;
            this.pathfinder = pathfinder;
            path = new List<Tile>();
            
            Initialize();
        }

        private void Initialize()
        {
            //Debug.Log("entity position: " + entity.transform.position);
            //Debug.Log("target position: " + patrolPoints[0].transform.position);
            path = pathfinder.FindPath(entity.transform.position, patrolPoints[0].transform.position);
            pathIndex = 0;
        }

        public void Process()
        {
            
            if(path != null && pathIndex < path.Count)
            {
                timer = 0.0f;
                Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
                entity.transform.position = Vector2.MoveTowards(entity.transform.position, targetPos, 2f * Time.deltaTime);


                Vector2 direction = targetPos - new Vector2(entity.transform.position.x, entity.transform.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                entity.transform.rotation = Quaternion.Euler(0, 0, angle);  

                if (Vector2.Distance(entity.transform.position, targetPos) < 0.1f)
                    pathIndex++;
            }
            else
            {
                if (timer < 4.0f)
                {
                    timer += Time.deltaTime;
                    lookAroundTimer -= Time.deltaTime;
                    if (lookAroundTimer < 0.0f)
                    {
                        lookAroundTimer = 0.8f;
                        Vector2 direction = MyMathUtils.GetRandomDirection();
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        entity.transform.rotation = Quaternion.Euler(0, 0, angle);
                    }

                } 
                else
                {
                    if (patrolPoints.Count == 0) return;

                    if (currentIndex >= patrolPoints.Count - 1)
                    {
                        currentIndex = 0;
                        path = pathfinder.FindPath(patrolPoints[patrolPoints.Count - 1].transform.position, patrolPoints[0].transform.position);
                        pathIndex = 0;
                        return;
                    }

                    currentIndex++;
                    path = pathfinder.FindPath(patrolPoints[currentIndex - 1].transform.position, patrolPoints[currentIndex].transform.position);
                    pathIndex = 0;
                    return;
                }
                
            } 
        }

        private void MoveTowards(Transform target)
        {
            Vector3 direction = (target.transform.position - entity.transform.position).normalized;

            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                entity.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            entity.transform.position += direction * patrolSpeed * Time.deltaTime;
        }

    }

    public class ArousedStrategy : IGuardStrategies
    {
        public event Action onSearchEnded;
        public event Action onRaccoonSpotted;

        readonly Transform entity;
        readonly Transform alertCue;
        private AStar pathfinder;
        private List<Tile> path;
        private int pathIndex = 0;
        readonly float searchSpeed = 2.0f;
        [SerializeField] private float arousedTime = 10.0f;
        private float timer = 0.0f;
        private float lookAroundTimer = 0.0f;
        private bool searching = false;

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


                Vector2 direction = targetPos - new Vector2(entity.transform.position.x, entity.transform.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                entity.transform.rotation = Quaternion.Euler(0, 0, angle);

                if (Vector2.Distance(entity.transform.position, targetPos) < 0.1f)
                    pathIndex++;
            }
            else
            {
                searching = true;
                timer += Time.deltaTime;
                lookAroundTimer += Time.deltaTime;
                if (lookAroundTimer > 2.0f)
                {
                    lookAroundTimer = 0f;
                    Vector2 direction = MyMathUtils.GetRandomDirection();
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    entity.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                if (timer > arousedTime)
                {
                    onSearchEnded?.Invoke();
                }
            }
        }

        public void RaccoonSpotted(Transform raccoonLoc)
        {
            if (searching)
                onRaccoonSpotted?.Invoke();
        }

        public void UpdatePlayerPosition(Transform playerPos, float radius)
        {
            if (Vector3.Distance(playerPos.position, entity.transform.position) <radius)
            {
                path = pathfinder.FindPath(entity.transform.position, playerPos.transform.position);
                pathIndex = 0;
            }
        }

    }

    public class CombatStrategy : IGuardStrategies
    {
        public event Action onCombatEnded;

        private Transform entity;
        private Transform player;
        private AStar pathfinder;
        private EnemyVisionCone visionCone;
        private List<Tile> path;
        private int pathIndex;
        private float attackSpeed;
        private float aStarTimer = 1.0f;
        private float combatTimer = 7.0f;
        float timer = 0.0f;

        public CombatStrategy(Transform entity, Transform player, AStar pathfinder, float attackSpeed)
        {
            this.entity = entity;
            this.player = player;
            this.pathfinder = pathfinder;
            this.attackSpeed = attackSpeed;
            Initialize();
        }

        public void Initialize()
        {
            path = pathfinder.FindPath(entity.transform.position, player.transform.position);
            pathIndex = 0;
            visionCone = entity.transform.GetComponent<EnemyVisionCone>();
            visionCone.playerUnobstructed += ResetCombatTimer;
        }

        public void Process()
        {
            aStarTimer -= Time.deltaTime;
            timer += Time.deltaTime;
            if (timer > combatTimer)
            {
                onCombatEnded?.Invoke();
            }
            if (aStarTimer < 0)
            {
                aStarTimer = 1.0f;
                Initialize();
            }

            if (path != null && pathIndex < path.Count)
            {
                Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
                entity.transform.position = Vector2.MoveTowards(entity.transform.position, targetPos, 2f * Time.deltaTime);


                Vector2 direction = targetPos - new Vector2(entity.transform.position.x, entity.transform.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                entity.transform.rotation = Quaternion.Euler(0, 0, angle);

                if (Vector2.Distance(entity.transform.position, targetPos) < 0.1f)
                    pathIndex++;
            }

            
        }

        void ResetCombatTimer()
        {
            timer = 0.0f; 
        }
    }

    public class SleepStrategy : IGuardStrategies
    {
        public event Action onSleepEnded;
        private readonly float sleepTimer = 10.0f;
        private float timer = 0.0f;

        public SleepStrategy()
        {

        }

        public void Process()
        {
            timer += Time.deltaTime;
            if (timer >= sleepTimer)
            {
                onSleepEnded?.Invoke();
            }
        }
    }
}