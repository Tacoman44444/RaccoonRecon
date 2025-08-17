using AI.BehaviorTrees;
using AI.Pathfinding;
using HelperFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UIElements;
using static AI.BehaviorTrees.Node;
using static UnityEngine.EventSystems.EventTrigger;

public class AIController
{
    private Transform agent;
    private Transform alertIndicator;
    private Transform combatIndicator;
    List<Transform> patrolPoints;
    private AStar pathfinder;
    private List<Tile> path;

    private int pathIndex = 0;
    private int currentIndex = 0;

    private bool reachedAlert;
    Vector2 lastAlert;

    private float searchDuration = 5.0f;
    private float searchTimer = 0.0f;
    private float lookInterval = 1.0f;
    private float lookTimer = 0.0f;

    private float seekPlayerInterval = 0.5f;
    private float seekPlayerTimer = 0.0f;

    public float detectPlayerInterval = 2.0f;

    private float patrolSpeed = 3.0f;
    private float alertSpeed = 3.5f;
    private float combatSpeed = 5.0f;

    private float combatDistance = 10.0f;
    private float killDistance = 1.0f;

    private float randomWalkTimer = 0.0f;
    private Vector2 walkDirection = Vector2.zero;

    private Vector3 alertOffset;
    private Vector3 combatOffset;

    public AIController(Transform agent, List<Transform> patrolPoints, AStar pathfinder, float patrolSpeed, float alertSpeed, float combatSpeed)
    {
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.patrolSpeed = patrolSpeed;
        this.pathfinder = pathfinder;
        this.alertSpeed = alertSpeed;
        this.combatSpeed = combatSpeed;
        alertIndicator = agent.transform.Find("AlertIndicator");
        combatIndicator = agent.transform.Find("CombatIndicator");
        alertOffset = alertIndicator.position - agent.transform.position;
        combatOffset = combatIndicator.position - agent.transform.position;
    }

    public Status PatrolAction()
    {
        if (path != null && pathIndex < path.Count)
        {
            Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
            agent.transform.position = Vector2.MoveTowards(agent.transform.position, targetPos, patrolSpeed * Time.deltaTime);


            Vector2 direction = targetPos - new Vector2(agent.transform.position.x, agent.transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            RotateSprite(angle);

            if (Vector2.Distance(agent.transform.position, targetPos) < 0.1f)
                pathIndex++;
        } 
        else
        {
            if (currentIndex >= patrolPoints.Count - 1)
            {
                currentIndex = 0;
                path = pathfinder.FindPath(agent.transform.position, patrolPoints[currentIndex].transform.position);
                pathIndex = 0;
            }
            else
            {
                currentIndex++;
                path = pathfinder.FindPath(agent.transform.position, patrolPoints[currentIndex].transform.position);
                pathIndex = 0;
            }

        }
        return Status.Running;
    }

    public void ResetPatrol()
    {
        currentIndex = 0;
    }

    public Status AlertAction(Vector2 alertCue)
    {

        if ((path == null && !reachedAlert) || alertCue != lastAlert)

        {
            lastAlert = alertCue;
            path = pathfinder.FindPath(agent.transform.position, alertCue);
            pathIndex = 0;
            return Status.Running;
        }
        else if (path != null)
        {
            Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
            agent.transform.position = Vector2.MoveTowards(agent.transform.position, targetPos, alertSpeed * Time.deltaTime);


            Vector2 direction = targetPos - new Vector2(agent.transform.position.x, agent.transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            RotateSprite(angle);

            if (Vector2.Distance(agent.transform.position, targetPos) < 0.1f)
                pathIndex++;
            if (pathIndex >= path.Count)
            {
                reachedAlert = true;
            }
            return Status.Running;
        } 
        else if (path == null && reachedAlert)
        {
            return Status.Success;
        }
        else
        {
            Debug.Log("ERROR::AlertAction() failed");
            return Status.Failure;
        }
    }

    public void ResetAlert()
    {
        reachedAlert = false;
    }

    public Node.Status SearchAction()
    {
        if (searchTimer > searchDuration)
        {
            return Status.Success;
        } else
        {
            searchTimer += Time.deltaTime;
            lookTimer += Time.deltaTime;
            if (lookTimer >= lookInterval)
            {
                lookTimer = 0f;

                Vector2 direction = MyMathUtils.GetRandomDirection();
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                RotateSprite(angle);
            }
            return Status.Running;
        }
    }

    public void ResetSearch()
    {
        searchTimer = 0f;
        lookTimer = 0f;
    }

    public Node.Status CombatAction(Transform player)
    {
        
        float dist = Vector2.Distance(player.transform.position, agent.transform.position);

        if (dist > combatDistance)
        {
            return Status.Failure;
        }

        if (dist < killDistance)
        {
            return Status.Success;
        }

        if (seekPlayerTimer > seekPlayerInterval)
        {
            path = pathfinder.FindPath(agent.transform.position, player.transform.position);
            pathIndex = 0;
            seekPlayerTimer = 0.0f;
        }
        else
        {
            if (path != null && pathIndex < path.Count)
            {
                Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
                agent.transform.position = Vector2.MoveTowards(agent.transform.position, targetPos, combatSpeed * Time.deltaTime);


                Vector2 direction = targetPos - new Vector2(agent.transform.position.x, agent.transform.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                RotateSprite(angle);

                if (Vector2.Distance(agent.transform.position, targetPos) < 0.1f)
                    pathIndex++;
            }
            seekPlayerTimer += Time.deltaTime;
        }
        return Status.Running;
    }

    public void ResetCombat()
    {
        seekPlayerTimer = 0.0f;

    }

    public Node.Status RandomWalk(float speed, float interval)
    {
        if (randomWalkTimer > interval)
        {
            randomWalkTimer = 0.0f;
            walkDirection = new Vector2(Random.value, Random.value).normalized;

        }
        float angle = Mathf.Atan2(walkDirection.y, walkDirection.x) * Mathf.Rad2Deg;
        RotateSprite(angle);
        agent.transform.position = Vector2.MoveTowards(agent.transform.position, agent.transform.position + new Vector3(walkDirection.x, walkDirection.y, 0.0f), speed * Time.deltaTime);
        randomWalkTimer += Time.deltaTime;
        return Status.Running;
    }

    public void ResetRandomWalk()
    {
        randomWalkTimer = 0.0f;
        walkDirection = Vector2.zero;
    }

    private void RotateSprite(float angle)
    {
        // Rotate the agent
        agent.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Restore indicators' world position so they don't orbit
        alertIndicator.position = agent.transform.position + alertOffset;
        combatIndicator.position = agent.transform.position + combatOffset;

        // Keep indicators upright (no rotation)
        alertIndicator.rotation = Quaternion.identity;
        combatIndicator.rotation = Quaternion.identity;
    }

}
