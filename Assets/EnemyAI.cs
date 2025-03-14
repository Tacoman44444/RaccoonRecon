using AI.FiniteStateMachine;
using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private StateMachine stateMachine;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float patrolSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.Log("[EnemyAI.cs]: No patrol points defined");
        }

        IStrategy patrolStrategy = new PatrolStrategy(transform, patrolPoints, patrolSpeed);

        State patrolState = new PatrolState("Patrol", patrolStrategy);

        // patrolState.AddTransition("EnemySpotted", attackState)

        Dictionary<string, State> allStates = new Dictionary<string, State>
        {
            {"Patrol", patrolState }
        };

        stateMachine = new StateMachine(patrolState, allStates);
       
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Process();
    }
}
