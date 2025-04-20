using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectiveData
{
    ObjectiveData() { }

    public int wheatCount;
    public int eggCount; 
    public int milkCount;


    public bool LevelComplete(int wheat, int eggs, int milk)
    {
        if (wheat >= wheatCount && eggs >= eggCount && milk >= milkCount) return true;
        return false;
    }
}

public struct Inventory
{
    public int wheat;
    public int eggs;
    public int milk;
    public int seeds;
}