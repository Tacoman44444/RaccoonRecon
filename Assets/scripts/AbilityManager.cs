using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum Abilities
{
    SLEEP_SPRAY,
    ANIMAL_AROUSE,
}

public class AbilityManager : MonoBehaviour
{
    public static Abilities activeAbility;
    [SerializeField] Image sleepSprayHighlight;
    [SerializeField] Image animalArouseHighlight;

    private void Start()
    {
        sleepSprayHighlight.transform.SetSiblingIndex(0);
        animalArouseHighlight.transform.SetSiblingIndex(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeAbility = Abilities.SLEEP_SPRAY;
            sleepSprayHighlight.transform.SetSiblingIndex(0);
            sleepSprayHighlight.gameObject.SetActive(true);
            animalArouseHighlight.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeAbility = Abilities.ANIMAL_AROUSE;
            animalArouseHighlight.gameObject.SetActive(true);
            sleepSprayHighlight.gameObject.SetActive(false);
        }
    }
}
