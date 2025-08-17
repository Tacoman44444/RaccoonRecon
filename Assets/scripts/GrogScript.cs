using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrogScript : MonoBehaviour
{
    private float nextXCoord = 0;
    private float YCoord = 0;
    private List<GameObject> grogImages = new List<GameObject>();
    [SerializeField] GameObject grogPrefab;
    private int startingGrogs = 3;

    private void Awake()
    {
        int i = 0;
        while (i < startingGrogs)
        {
            i++;
            SpawnNewGrogImage();
        }
    }

    public bool SpawnNewGrogImage()
    {
        GameObject newGrog = GameObject.Instantiate(grogPrefab, transform);
        RectTransform rect = newGrog.GetComponent<RectTransform>();
        // Middle-Center
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        // Optional: reset position so it's centered
        rect.sizeDelta = new Vector2(30, 30);
        rect.anchoredPosition = new Vector2(nextXCoord, YCoord);
        nextXCoord += 50;
        grogImages.Add(newGrog); ;
        return true;

    }

    public bool RemoveGrogImage()
    {
        if (grogImages.Count > 0)
        {
            GameObject lastItem = grogImages[grogImages.Count - 1];
            grogImages.RemoveAt(grogImages.Count - 1);
            Destroy(lastItem);
            nextXCoord -= 50;
            return true;
        }
        else
        {
            return false;
        }
        
    }

}
