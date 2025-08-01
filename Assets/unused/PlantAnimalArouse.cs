using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimalArouse : MonoBehaviour
{
    [SerializeField] private GameObject animalArousePrefab;
    private static int instances = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && AbilityManager.activeAbility == Abilities.ANIMAL_AROUSE && instances > 0)
        {
            Instantiate(animalArousePrefab, transform.position, Quaternion.identity);
            instances--;
        }
    }
}
