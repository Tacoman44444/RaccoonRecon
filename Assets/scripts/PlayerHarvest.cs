using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HarvestEventManager.Harvest();
            Debug.Log("efuef");
        }
    }
}
