using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimalArouse : MonoBehaviour
{
    private float soundCueRange = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (Vector3.Distance(transform.position, enemies[i].transform.position) < soundCueRange)
                {
                    enemies[i].SoundCue(transform);
                }
            }
            Destroy(gameObject);
        }
    }
}
