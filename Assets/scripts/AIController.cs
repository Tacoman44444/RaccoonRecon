using AI.BehaviorTrees;
using AI.Pathfinding;
using HelperFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using static AI.BehaviorTrees.Node;
using static UnityEngine.EventSystems.EventTrigger;

public class AIController
{
    private Transform agent;
    List<Transform> patrolPoints;
    private AStar pathfinder;
    private List<Tile> path;

    private int pathIndex = 0;
    private int currentIndex = 0;

    private bool reachedAlert;

    private float searchDuration = 5.0f;
    private float searchTimer = 0.0f;
    private float lookInterval = 1.0f;
    private float lookTimer = 0.0f;

    private float seekPlayerInterval = 0.5f;
    private float seekPlayerTimer = 0.0f;

    private float patrolSpeed;
    private float alertSpeed;
    private float combatSpeed;

    public AIController(Transform agent, List<Transform> patrolPoints, float patrolSpeed, float alertSpeed, float combatSpeed)
    {
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.patrolSpeed = patrolSpeed;
        this.alertSpeed = alertSpeed;
        this.combatSpeed = combatSpeed;

    }

    public Status PatrolAction()
    {
        if (path != null && pathIndex < path.Count)
        {
            Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
            agent.transform.position = Vector2.MoveTowards(agent.transform.position, targetPos, patrolSpeed * Time.deltaTime);


            Vector2 direction = targetPos - new Vector2(agent.transform.position.x, agent.transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            agent.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (Vector2.Distance(agent.transform.position, targetPos) < 0.1f)
                pathIndex++;
        } 
        else
        {
            if (currentIndex >= patrolPoints.Count - 1)
            {
                currentIndex = 0;
                path = pathfinder.FindPath(patrolPoints[patrolPoints.Count - 1].transform.position, patrolPoints[0].transform.position);
                pathIndex = 0;
            }
            else
            {
                currentIndex++;
                path = pathfinder.FindPath(patrolPoints[currentIndex - 1].transform.position, patrolPoints[currentIndex].transform.position);
                pathIndex = 0;
            }

        }
        return Status.Running;
    }

    public void ResetPatrol()
    {
        pathIndex = 0;
        currentIndex = 0;
        path = null;
    }

    public Status AlertAction(Transform alertCue)
    {
        if (path == null && !reachedAlert)
        {
            path = pathfinder.FindPath(agent.transform.position, alertCue.transform.position);
            pathIndex = 0;
            return Status.Running;
        }
        else if (path != null)
        {
            Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
            agent.transform.position = Vector2.MoveTowards(agent.transform.position, targetPos, alertSpeed * Time.deltaTime);


            Vector2 direction = targetPos - new Vector2(agent.transform.position.x, agent.transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            agent.transform.rotation = Quaternion.Euler(0, 0, angle);

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
        path = null;
        pathIndex = 0; 
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
                agent.transform.rotation = Quaternion.Euler(0, 0, angle);
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
                agent.transform.rotation = Quaternion.Euler(0, 0, angle);

                if (Vector2.Distance(agent.transform.position, targetPos) < 0.1f)
                    pathIndex++;
            }
        }
        return Status.Running;
    }

    public void ResetCombat()
    {
        seekPlayerTimer = 0.0f;
        path = null;
        pathIndex = 0;

    }

}
