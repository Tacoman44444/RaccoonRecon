using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovementScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float sneakSpeed = 1.0f;
    [SerializeField] private float runSpeed = 2.0f;
    public InputAction playerControls;
    private Vector2 moveDirection = Vector2.zero;
    private float speedDampingFactor = 100.0f;

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
        sneakSpeed /= speedDampingFactor;
        runSpeed /= speedDampingFactor;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = playerControls.ReadValue<Vector2>();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += new Vector3(moveDirection.x * runSpeed, moveDirection.y * runSpeed, 0.0f);
        }
        else
        {
            transform.position += new Vector3(moveDirection.x * sneakSpeed, moveDirection.y * sneakSpeed, 0.0f);
        }
    }
}
