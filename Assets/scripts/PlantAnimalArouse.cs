using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimalArouse : MonoBehaviour
{
    [SerializeField] private GameObject animalArousePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(animalArousePrefab, transform.position, Quaternion.identity);
        }
    }
}
