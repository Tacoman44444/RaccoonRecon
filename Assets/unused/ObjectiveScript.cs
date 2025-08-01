using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveScript : MonoBehaviour
{
    [SerializeField] private ObjectiveData objectiveData;

    [SerializeField] private TextMeshProUGUI maxWheatGUI;
    [SerializeField] private TextMeshProUGUI maxEggsGUI;
    [SerializeField] private TextMeshProUGUI maxMilkGUI;


    [SerializeField] private TextMeshProUGUI wheatCount;
    [SerializeField] private TextMeshProUGUI eggsCount;
    [SerializeField] private TextMeshProUGUI milkCount;
    [SerializeField] private TextMeshProUGUI seedsCount;


    [SerializeField] Canvas objectiveCanvas;
    [SerializeField] private GameObject homebase;

    [SerializeField] LayerMask playerLayer;

    private Inventory inventory;
    private Vector3 homebasePosition;

    private void OnEnable()
    {
        WheatHarvested.OnWheatDestroyed += IncrementWheat;
        HarvestEventManager.OnEggHarvest += IncrementEgg;
        CowProduce.OnCowHarvest += IncrementMilk;
    }

    private void OnDisable()
    {
        WheatHarvested.OnWheatDestroyed -= IncrementWheat;
        HarvestEventManager.OnEggHarvest -= IncrementEgg;
        CowProduce.OnCowHarvest -= IncrementMilk;
    }

    void Start()
    {
        homebasePosition = homebase.transform.position;
        homebasePosition.z = 0;
        Debug.Log("homebase position: " + homebasePosition);
        inventory.wheat = 0;
        inventory.eggs = 0;
        inventory.milk = 0;
        inventory.seeds = 0;

        maxWheatGUI.text = "> Wheat: " + objectiveData.wheatCount;
        maxEggsGUI.text = "> Eggs: " + objectiveData.eggCount;
        maxMilkGUI.text = "> Milk: " + objectiveData.milkCount;
    }

    void Update()
    {
        if (Vector3.Distance(homebasePosition, transform.position) < 3.0f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                objectiveCanvas.gameObject.SetActive(!objectiveCanvas.gameObject.activeInHierarchy);
            }
        }
        else if (Vector3.Distance(homebasePosition, transform.position) > 5.0f)
        {
            objectiveCanvas.gameObject.SetActive(false);
        }
    }

    public void CheckMissionComplete()
    {
        if (objectiveData.LevelComplete(inventory.wheat, inventory.eggs, inventory.milk))
        {
            Debug.Log("Mission Complete!");
        }
        else
        {
            Debug.Log("Hiding");
        }
    }

    public void IncrementWheat()
    {
        inventory.wheat++;
        wheatCount.text = inventory.wheat.ToString();
    }

    public void IncrementWheat(Vector3Int vec)
    {
        inventory.wheat++;
        wheatCount.text = inventory.wheat.ToString();
    }

    public bool DecreaseWheat(int num)
    {
        if (inventory.wheat >= num)
        {
            inventory.wheat -= num;
            wheatCount.text = inventory.wheat.ToString();
            return true;
        }
        else
        {
            Debug.Log("Not enough!");
            return false;
        }
        
    }

    public bool DecrementSeeds()
    {
        if (inventory.seeds > 0)
        {
            inventory.seeds--;
            seedsCount.text = inventory.seeds.ToString();
            return true;
        }
        else
        {
            Debug.Log("Not enough seeds to plant more wheat");
            return false;
        }
    }

    public void IncrementEgg()
    {
        inventory.eggs++;
        eggsCount.text = inventory.eggs.ToString();
    }

    public void IncrementMilk()
    {
        inventory.milk++;
        milkCount.text = inventory.milk.ToString();
    }

    public void AddSeeds(int num)
    {
        if (inventory.seeds <= 100)
        {
            inventory.seeds += num;
            seedsCount.text = inventory.seeds.ToString();
        }
    }

}
