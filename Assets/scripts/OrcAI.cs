using AI.BehaviorTrees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAI : MonoBehaviour
{
    private BehaviorTree tree;
    private AIController controller;
    [SerializeField] List<Transform> patrolPoints;
    void Awake()
    {
        tree = new BehaviorTree("Orc");
        controller = new AIController(transform, patrolPoints, 2.0f, 2.5f, 3.2f);

    }

    // Update is called once per frame
    void Update()
    {
        tree.Process();
    }
}
