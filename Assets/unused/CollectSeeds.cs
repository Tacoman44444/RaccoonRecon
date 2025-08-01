using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSeeds : MonoBehaviour
{
    [SerializeField] private GameObject barn;
    private ObjectiveScript objectiveScript;

    private void Start()
    {
        objectiveScript = GetComponent<ObjectiveScript>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Vector3.Distance(transform.position, barn.transform.position) < 3.0f)
            {
                objectiveScript.AddSeeds(10);
                Debug.Log("Collected seeds");
            }
        }
    }
}
