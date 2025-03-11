using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class eggSpawnScript : MonoBehaviour
{
    [SerializeField] int eggSpawnSparsity = 3;
    [SerializeField] float eggSpawnInterval = 10.0f;
    [SerializeField] GameObject eggPrefab;
    private float timer = 0.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > eggSpawnInterval)
        {
            timer = 0.0f;
            SpawnEgg();
        }        
    }

    void SpawnEgg()
    {
        int randomValue = Random.Range(1, eggSpawnSparsity);
        if (randomValue == 1)
        {
            Instantiate(eggPrefab, transform.position, Quaternion.identity);
        }
    }
}
