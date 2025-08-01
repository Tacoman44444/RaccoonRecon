using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheatHarvested : MonoBehaviour
{
    public static event Action<Vector3Int> OnWheatDestroyed;
    private float soundCueRange = 10.0f;
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
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (Vector3.Distance(transform.position, enemies[i].transform.position) < soundCueRange)
                {
                    enemies[i].SoundCue(transform);
                }
            }
            OnWheatDestroyed?.Invoke(new Vector3Int(Convert.ToInt32(transform.position.x - 0.5f), Convert.ToInt32(transform.position.y - 0.5f), 0));
            Destroy(gameObject);
        }
    }
}
