using AI.BehaviorTrees;
using AI.Pathfinding;
using BlackboardSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class HostageAI : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GridWorld world;

    private GameObject spawnPoint;
    private BehaviorTree tree;
    readonly Blackboard blackboard = new Blackboard();
    private BlackboardKey detected;
    private BlackboardKey following;
       

    void Awake()
    {
        tree = new BehaviorTree("hostage");
        detected = blackboard.GetOrRegisterKey("IsDetected");
        following = blackboard.GetOrRegisterKey("IsFollowing");
        blackboard.SetValue(detected, false);
        blackboard.SetValue(following, false);

        spawnPoint = new GameObject("Hostage Spawn Point");
        spawnPoint.transform.position = transform.position;

        Leaf flee = new Leaf("Flee", new FleeCommand(transform, spawnPoint.transform, blackboard, new AStar(world)));
        Leaf follow = new Leaf("Follow", new FollowCommand(transform, player, blackboard, new AStar(world)));
        Leaf enemyDetected = new Leaf("EnemyDetected", new Condition(isDetected));
        Leaf followingPlayer = new Leaf("FollowingPlayer", new Condition(isFollowing));

        Sequence fleeSequence = new Sequence("FleeSequence", 100);
        fleeSequence.AddChild(enemyDetected);
        fleeSequence.AddChild(flee);
        Sequence followSequence = new Sequence("FollowSequence", 50);
        followSequence.AddChild(followingPlayer);
        followSequence.AddChild(follow);

        PrioritySelector rootSelector = new PrioritySelector("RootSelector");
        rootSelector.AddChild(fleeSequence);
        rootSelector.AddChild(followSequence);

        tree.AddChild(rootSelector);
    }


    void Update()
    {
        tree.Process();
        if (Vector3.Distance(transform.position, player.transform.position) < 0.5f)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (blackboard.TryGetValue(following, out bool val))
                {
                    blackboard.SetValue(following, !val);
                    Debug.Log("Changed follow state to: " + !val);
                }
            }
        }
    }

    bool isDetected()
    {
        if (blackboard.TryGetValue<bool>(detected, out bool val))
        {
            return val;
        }
        Debug.Log("ERROR::blackboard did not work");
        return false;
    }

    bool isFollowing()
    {
        if (blackboard.TryGetValue<bool>(following, out bool val))
        {
            Debug.Log("Value of following now is: " + val);
            return val;
        }
        Debug.Log("ERROR::blackboard did not work");
        return false;
    }

    public Blackboard GetBlackboard()
    {
        return this.blackboard;
    }
}

public class FleeCommand : ICommand
{
    private Transform hostage;
    private Transform spawn;
    private Blackboard blackboard;
    private List<Tile> path;
    private AStar pathfinder;
    private int pathIndex = 0;
    private float fleeSpeed = 10.0f;
    private bool createdPath = false;
    
    public FleeCommand(Transform hostage, Transform spawn, Blackboard blackboard, AStar pathfinder)
    {
        this.hostage = hostage;
        this.spawn = spawn;
        this.pathfinder = pathfinder;
        this.path = new List<Tile>();
    }
    public Node.Status Process()
    {
        if (!createdPath)
        {
            Debug.Log("Hostage position: " + hostage.transform.position);
            Debug.Log("Spawn position" + spawn.transform.position);
            path = pathfinder.FindPath(hostage.transform.position, spawn.transform.position);
            pathIndex = 0;
            createdPath = true;
            return Node.Status.Running;
        }
        else
        {
            if (pathIndex >= path.Count)
            {
                Debug.Log("SUCCESS");
                return Node.Status.Success;
            } else
            {
                Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
                hostage.transform.position = Vector2.MoveTowards(hostage.transform.position, targetPos, fleeSpeed * Time.deltaTime);

                Vector2 direction = targetPos - new Vector2(hostage.transform.position.x, hostage.transform.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                hostage.transform.rotation = Quaternion.Euler(0, 0, angle);

                if (Vector2.Distance(hostage.transform.position, targetPos) < 0.1f)
                    pathIndex++;
            }

            return Node.Status.Running;
        }
    }

    public void Reset()
    {
        createdPath = false;
    }
}

public class FollowCommand : ICommand
{
    private Transform hostage;
    private Transform player;
    private Blackboard blackboard;
    private BlackboardKey following;
    private List<Tile> path = new List<Tile>();
    private AStar pathfinder;
    private int pathIndex = 0;
    private float followSpeed = 2.0f;

    public FollowCommand(Transform hostage, Transform player, Blackboard blackboard, AStar pathfinder)
    {
        this.hostage = hostage;
        this.player = player;
        this.blackboard = blackboard;
        this.pathfinder = pathfinder;
        this.following = blackboard.GetOrRegisterKey("IsFollowing");
    }

    public Node.Status Process()
    {
        if (!blackboard.TryGetValue(following, out bool val) || !val)
        {
            return Node.Status.Failure;
        }
        path = pathfinder.FindPath(hostage.transform.position, player.transform.position);
        if (path.Count <= pathIndex)
        {
            return Node.Status.Running;
        }
        pathIndex = 0;
        Vector2 targetPos = new Vector2(path[pathIndex].worldPosition.x, path[pathIndex].worldPosition.y);
        hostage.transform.position = Vector2.MoveTowards(hostage.transform.position, targetPos, followSpeed * Time.deltaTime);

        Vector2 direction = targetPos - new Vector2(hostage.transform.position.x, hostage.transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        hostage.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector2.Distance(hostage.transform.position, targetPos) < 0.1f)
            pathIndex++;

        return Node.Status.Running;

    }

}