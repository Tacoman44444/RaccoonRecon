using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovementScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    [SerializeField] private float sneakSpeed = 10.0f;
    [SerializeField] private float runSpeed = 20.0f;
    public InputAction playerControls;
    private Vector2 moveDirection = Vector2.zero;

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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = new Vector3(moveDirection.x * runSpeed, moveDirection.y * runSpeed, 0.0f);
        }
        else
        {
            rb.velocity = new Vector3(moveDirection.x * sneakSpeed, moveDirection.y * sneakSpeed, 0.0f);
        }
    }
}
