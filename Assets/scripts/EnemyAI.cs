using AI.FiniteStateMachine;
using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Pathfinding;

public class EnemyAI : MonoBehaviour
{

    private StateMachine stateMachine;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private GridWorld gridWorld;
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float searchSpeed = 3.0f;
    private Dictionary<(string, State), State> transitionTable = new Dictionary<(string, State), State>();
    private EnemyVisionCone visionCone;
    private AStar pathfinder;

    private void Awake()
    {
        visionCone = GetComponent<EnemyVisionCone>();
    }

    private void OnEnable()
    {
        visionCone.OnPlayerSensed += RaccoonSensed;
    }

    private void OnDisable()
    {
        visionCone.OnPlayerSensed -= RaccoonSensed;
    }

    // Start is called before the first frame update
    void Start()
    {

        //gridWorld = FindObjectOfType<GridWorld>();
        pathfinder = new AStar(gridWorld);
        if (patrolPoints.Count == 0)
        {
            Debug.Log("[EnemyAI.cs]: No patrol points defined");
        }

        IStrategy patrolStrategy = new PatrolStrategy(transform, patrolPoints, patrolSpeed);
        IStrategy arousedStrategy = new ArousedStrategy(transform, transform, pathfinder, patrolSpeed);

        State patrolState = new PatrolState("Patrol", patrolStrategy);
        State arousedState = new ArousedState("Aroused", arousedStrategy);

        // patrolState.AddTransition("EnemySpotted", attackState)
        transitionTable.Add(("RaccoonSensed", patrolState), arousedState);

        stateMachine = new StateMachine(patrolState, transitionTable);
       
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Process();
    }

    void RaccoonSensed(Transform alertCue)
    {
        Debug.Log("Raccoon was sensed"); // code is working upto and including this command
        stateMachine.ChangeState("RaccoonSensed", new ArousedStrategy(transform, alertCue, pathfinder, searchSpeed));
    }

}
