using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DirtTileData
{
    public Vector3Int position;
    public bool isPlanted;
}

public class InitializeWheatCrop : MonoBehaviour
{
    [SerializeField] private Tilemap wheatCropMap;
    [SerializeField] private GameObject wheatPrefab;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private Transform player;
    private ObjectiveScript objectiveScript;

    // List<DirtTileData> dirtTiles = new List<DirtTileData>();
    private Dictionary<Vector3Int, bool> dirtTiles = new Dictionary<Vector3Int, bool>();
    private float probability = 0.5f;

    private void OnEnable()
    {
        WheatHarvested.OnWheatDestroyed += ResetWheatSwitch;
    }

    private void OnDisable()
    {
        WheatHarvested.OnWheatDestroyed -= ResetWheatSwitch;
    }

    void Start()
    {
        objectiveScript = player.GetComponent<ObjectiveScript>();
        dirtTiles = GetAllTiles(wheatCropMap);
        foreach (Vector3Int tilePos in dirtTiles.Keys.ToList())
        {
            float num = Random.Range(0.0f, 1.0f);
            if (num > probability)
            {
                Instantiate(wheatPrefab, new Vector3((float)(tilePos.x + 0.5f), (float)(tilePos.y + 0.5f), 0), Quaternion.identity);
                dirtTiles[tilePos] = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            foreach (Vector3Int tilePos in dirtTiles.Keys.ToList())
            {
                if (Vector3.Distance(tilePos, player.transform.position) < 2.0f && !dirtTiles[tilePos])
                {
                    if (objectiveScript.DecrementSeeds())
                    {
                        Instantiate(seedPrefab, new Vector3((float)(tilePos.x + 0.5f), (float)(tilePos.y + 0.5f), 0), Quaternion.identity);
                        dirtTiles[tilePos] = true;
                    }
                }
            }
        }
    }

    public void ResetWheatSwitch(Vector3Int tilePosition)
    {
        if (dirtTiles.ContainsKey(tilePosition))
        {
            dirtTiles[tilePosition] = false;
            Debug.Log("ResetWheatSwitch ran successfully lesogo");
        }
        else
        {
            Debug.Log("ResetWheatSwitch    Shit");
        }
    }

    private Dictionary<Vector3Int, bool> GetAllTiles(Tilemap tilemap)
    {
        Dictionary <Vector3Int, bool> dict = new Dictionary <Vector3Int, bool>();

        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePosition);
                if (tile != null)
                {
                    dict[tilePosition] = false;
                }
            }
        }

        return dict;
    }
}
