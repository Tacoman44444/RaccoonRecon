using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestEventManager : MonoBehaviour
{
    public static event Action OnWheatHarvest;
    public static event Action OnEggHarvest;

    public static void WheatHarvest()
    {
        OnWheatHarvest?.Invoke();
    }

    public static void EggHarvest()
    {
        OnEggHarvest?.Invoke();
    }
}
