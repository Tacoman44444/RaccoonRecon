using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class playerMovementScript : MonoBehaviour
{
    public event Action<Transform, float> onPlayerMoved;

    [SerializeField] private List<Tilemap> groundTilemaps = new List<Tilemap>();
    private Dictionary<string, float> groundSoundMultipliers = new Dictionary<string, float>();

    private Rigidbody2D rb;
    [SerializeField] private float sneakSpeed = 10.0f;
    [SerializeField] private float runSpeed = 20.0f;
    public InputAction playerControls;
    private Vector2 moveDirection = Vector2.zero;
    private float checkTileTimer = 0.5f;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundSoundMultipliers["Grass"] = 0.5f;
        groundSoundMultipliers["WetDirt"] = 1.5f;
        groundSoundMultipliers["Wheat"] = 1.2f;
        groundSoundMultipliers["Bushes"] = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = playerControls.ReadValue<Vector2>();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = new Vector3(moveDirection.x * runSpeed, moveDirection.y * runSpeed, 0.0f);
        }
        else
        {
            rb.velocity = new Vector3(moveDirection.x * sneakSpeed, moveDirection.y * sneakSpeed, 0.0f);
        }
        if (moveDirection != Vector2.zero)
        {
            checkTileTimer += Time.deltaTime;
            if (checkTileTimer >= 0.5f)
            {
                Tilemap groundTile = CheckForTile();
                if (groundSoundMultipliers.ContainsKey(groundTile.tag))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        onPlayerMoved?.Invoke(transform, groundSoundMultipliers[groundTile.tag] * 3);
                        Debug.Log("Invoked player moved");
                    }
                    else
                    {
                        onPlayerMoved?.Invoke(transform, groundSoundMultipliers[groundTile.tag]);
                        Debug.Log("Invoked player moved");
                    }
                }
                checkTileTimer = 0.0f;

            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    private Tilemap CheckForTile()
    {
        foreach (Tilemap tilemap in groundTilemaps)
        {
            Vector3Int tilePos = tilemap.WorldToCell(transform.position);
            TileBase tile = tilemap.GetTile(tilePos);

            if (tile != null)
            {
                return tilemap;
            }
        }

        return null;
    }
}
