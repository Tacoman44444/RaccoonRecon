using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheatHarvested : MonoBehaviour
{
    private void OnEnable()
    {
        HarvestEventManager.OnHarvest += HarvestWheat;
    }

    // Update is called once per frame
    private void OnDisable()
    {
        HarvestEventManager.OnHarvest -= HarvestWheat;
    }

    void HarvestWheat()
    {
        Debug.Log("RUNNING");
        if (Vector3.Distance(transform.position, FindObjectOfType<PlayerHarvest>().transform.position) < 2.0f)
        {
            Debug.Log("Wheat Destroyed!");
            Destroy(gameObject);
        }
    }
}
