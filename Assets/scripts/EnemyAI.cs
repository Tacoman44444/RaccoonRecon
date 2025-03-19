using AI.FiniteStateMachine;
using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Pathfinding;

public class EnemyAI : MonoBehaviour
{

    private StateMachine stateMachine;
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private GridWorld gridWorld;
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float searchSpeed = 3.0f;
    [SerializeField] private float combatSpeed = 5.0f;
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
        visionCone.OnPlayerSpotted += RaccoonSpotted;
    }

    private void OnDisable()
    {
        visionCone.OnPlayerSensed -= RaccoonSensed;
        visionCone.OnPlayerSpotted -= RaccoonSpotted;
    }

    // Start is called before the first frame update
    void Start()
    {

        //gridWorld = FindObjectOfType<GridWorld>();
        if (patrolPoints.Count == 0)
        {
            Debug.Log("[EnemyAI.cs]: No patrol points defined");
        }

        IStrategy arousedStrategy = new ArousedStrategy(transform, transform, new AStar(gridWorld), patrolSpeed);
        IStrategy patrolStrategy = new PatrolStrategy(transform, patrolPoints, new AStar(gridWorld), searchSpeed);
        IStrategy combatStrategy = new CombatStrategy(transform, transform, new AStar(gridWorld), combatSpeed);

        State arousedState = new ArousedState("Aroused", arousedStrategy);
        State patrolState = new PatrolState("Patrol", patrolStrategy);
        State combatState = new CombatState("Combat", combatStrategy);


        // patrolState.AddTransition("EnemySpotted", attackState)
        transitionTable.Add(("RaccoonSensed", patrolState), arousedState);
        transitionTable.Add(("SearchEnded", arousedState), patrolState);
        transitionTable.Add(("SoundCue", patrolState), arousedState);
        transitionTable.Add(("RaccoonSpotted", arousedState), combatState);
        transitionTable.Add(("CombatEnded", combatState), arousedState);

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
        ArousedStrategy arousedStrategy = new ArousedStrategy(transform, alertCue, new AStar(gridWorld), searchSpeed);
        stateMachine.ChangeState("RaccoonSensed", arousedStrategy);
        arousedStrategy.onSearchEnded += SearchEnded; 
        //kind of a shithouse way to do it, because I have to resubscribe SearchEnded() to the current arousedstrategy's onSearchEnabled event. I could
        //perhaps have a 'timer expired' event happen anytime a state is running for long enough, and then catch it in the transition table as needed.

    }

    void SearchEnded() {
        Debug.Log("Search Ended");
        stateMachine.ChangeState("SearchEnded", new PatrolStrategy(transform, patrolPoints, new AStar(gridWorld), patrolSpeed));
    }

    void CombatEnded()
    {
        Debug.Log("Combat Ended");
        ArousedStrategy arousedStrategy = new ArousedStrategy(transform, transform, new AStar(gridWorld), searchSpeed);
        stateMachine.ChangeState("CombatEnded", arousedStrategy);
        arousedStrategy.onSearchEnded += SearchEnded;
        
    }

    public void SoundCue(Transform alertCue)
    {
        Debug.Log("Sound cue was heard");
        ArousedStrategy arousedStrategy = new ArousedStrategy(transform, alertCue, new AStar(gridWorld), searchSpeed);
        stateMachine.ChangeState("SoundCue", arousedStrategy);
        arousedStrategy.onSearchEnded += SearchEnded;
    }

    public void RaccoonSpotted()
    {
        Debug.Log("Racoon was spotted");
        CombatStrategy combatStrategy = new CombatStrategy(transform, player, new AStar(gridWorld), combatSpeed);
        stateMachine.ChangeState("RaccoonSpotted", combatStrategy);
        combatStrategy.onCombatEnded += CombatEnded;
    }

}
