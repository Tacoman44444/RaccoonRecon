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
    public Animator animator;

    private Rigidbody2D rb;
    [SerializeField] private float sneakSpeed = 10.0f;
    [SerializeField] private float runSpeed = 20.0f;
    public InputAction playerControls;
    private Vector2 moveDirection = Vector2.zero;
    private float checkTileTimer = 0.5f;
    private float runSound = 3.0f;
    private float walkSound = 1.0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = playerControls.ReadValue<Vector2>();
        animator.SetInteger("horizontal_direction", Convert.ToInt32(moveDirection.x));
        animator.SetInteger("vertical_direction", Convert.ToInt32(moveDirection.y));


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
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    onPlayerMoved?.Invoke(transform, runSound);
                    Debug.Log("Invoked player moved");
                }
                else
                {
                    onPlayerMoved?.Invoke(transform, walkSound);
                    Debug.Log("Invoked player moved");

                }
                checkTileTimer = 0.0f;

            }
        }
    }
}
