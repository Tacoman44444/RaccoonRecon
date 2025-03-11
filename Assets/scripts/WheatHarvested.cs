using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheatHarvested : MonoBehaviour
{
    private void OnEnable()
    {
        HarvestEventManager.OnWheatHarvest += HarvestWheat;
    }

    // Update is called once per frame
    private void OnDisable()
    {
        HarvestEventManager.OnWheatHarvest -= HarvestWheat;
    }

    void HarvestWheat()
    {
        if (Vector3.Distance(transform.position, FindObjectOfType<PlayerHarvest>().transform.position) < 2.0f)
        {
            Destroy(gameObject);
        }
    }
}
