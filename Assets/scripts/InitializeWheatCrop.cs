using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InitializeWheatCrop : MonoBehaviour
{
    [SerializeField] private Tilemap wheatCropMap;
    [SerializeField] private GameObject wheatPrefab; 
    // Start is called before the first frame update
    void Start()
    {
        List<Vector3Int> dirtTiles = GetAllTiles(wheatCropMap);
        foreach (Vector3Int tilePos in dirtTiles)
        {
            Instantiate(wheatPrefab, new Vector3((float)(tilePos.x + 0.5f), (float)(tilePos.y + 0.5f), 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Vector3Int> GetAllTiles(Tilemap tilemap)
    {
        List<Vector3Int> tilePositions = new List<Vector3Int>();

        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePosition);
                if (tile != null)
                {
                    tilePositions.Add(tilePosition);
                }
            }
        }

        return tilePositions;
    }
}
