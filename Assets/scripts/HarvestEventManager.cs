using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestEventManager : MonoBehaviour
{
    public static event Action OnHarvest;

    public static void Harvest()
    {
        Debug.Log("Harvest() called");
        OnHarvest?.Invoke();
    }
}
