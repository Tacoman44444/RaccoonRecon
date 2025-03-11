using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggHarvested : MonoBehaviour
{
    private void OnEnable()
    {
        HarvestEventManager.OnEggHarvest += HarvestEgg;
    }

    private void OnDisable()
    {
        HarvestEventManager.OnEggHarvest -= HarvestEgg;
    }

    void HarvestEgg()
    {
        if (Vector3.Distance(transform.position, FindObjectOfType<PlayerHarvest>().transform.position) < 2.0f)
        {
            Destroy(gameObject);
        }
    }
}

