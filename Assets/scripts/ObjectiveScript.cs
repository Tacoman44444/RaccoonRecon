using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveScript : MonoBehaviour
{
    [SerializeField] private ObjectiveData objectiveData;

    [SerializeField] private TextMeshProUGUI wheatGUI;
    [SerializeField] private TextMeshProUGUI eggsGUI;
    [SerializeField] private TextMeshProUGUI milkGUI;

    [SerializeField] Canvas objectiveCanvas;
    [SerializeField] private GameObject homebase;

    [SerializeField] LayerMask playerLayer;

    private Inventory inventory;
    private Vector3 homebasePosition;

    private void OnEnable()
    {
        WheatHarvested.OnWheatDestroyed += IncrementWheat;
        HarvestEventManager.OnEggHarvest += IncrementEgg;
    }

    private void OnDisable()
    {
        WheatHarvested.OnWheatDestroyed -= IncrementWheat;
        HarvestEventManager.OnEggHarvest -= IncrementEgg;
    }

    void Start()
    {
        homebasePosition = homebase.transform.position;
        homebasePosition.z = 0;
        Debug.Log("homebase position: " + homebasePosition);
        inventory.wheat = 0;
        inventory.eggs = 0;
        inventory.milk = 0;

        wheatGUI.text = "> Wheat: " + objectiveData.wheatCount;
        eggsGUI.text = "> Eggs: " + objectiveData.eggCount;
        milkGUI.text = "> Milk: " + objectiveData.milkCount;
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
        Debug.Log("wheat in inventory: " + inventory.wheat);
    }

    public void IncrementEgg()
    {
        inventory.eggs++;
        Debug.Log("eggs in inventory: " + inventory.eggs);
    }

    public void IncrementMilk()
    {
        inventory.milk++;
    }

}
