using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CowState
{
    COW_UNFED,
    COW_FED,
    COW_READYTOMILK,
}

public class CowProduce : MonoBehaviour
{
    public Transform player;
    public CowState state = CowState.COW_UNFED;

    private float TIME_TO_MILK = 5.0f;
    private float timer = 0.0f;
    private ObjectiveScript objectiveScript;

    private void Start()
    {
        objectiveScript = player.GetComponent<ObjectiveScript>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 1.0f)
            {
                if (state == CowState.COW_UNFED)
                {
                    if (objectiveScript.DecreaseWheat(4)) {
                        state = CowState.COW_FED;
                    }
                }
                if (state == CowState.COW_READYTOMILK)
                {
                    objectiveScript.IncrementMilk();
                    state = CowState.COW_UNFED;
                }
            }
        }
        if (state == CowState.COW_FED)
        {
            timer += Time.deltaTime;
            if (timer > TIME_TO_MILK)
            {
                timer = 0.0f;
                state = CowState.COW_READYTOMILK;
            }
        }
        else
        {
            timer = 0.0f;
        }

    }


}
