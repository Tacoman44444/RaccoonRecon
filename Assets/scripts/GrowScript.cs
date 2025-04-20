using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowScript : MonoBehaviour
{
    [SerializeField] GameObject wheatPrefab;
    public float growTime;
    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > growTime)
        {
            Instantiate(wheatPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
