using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "CustomTiles/TorchTile")]
public class TorchTile : Tile
{
    public GameObject lightPrefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (lightPrefab != null)
        {
            Tilemap map = tilemap.GetComponent<Tilemap>();

            Vector3 worldPos = map.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0);

            GameObject lightObj = GameObject.Instantiate(lightPrefab, worldPos, Quaternion.identity);

            lightObj.transform.SetParent(map.transform, true);
        }

        // Always return base.StartUp to keep Unity happy
        return base.StartUp(position, tilemap, go);
    }
}
