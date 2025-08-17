using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class playerMovementScript : MonoBehaviour
{
    public Animator animator;

    private Rigidbody2D rb;
    [SerializeField] GameObject grogPrefab;
    [SerializeField] private float sneakSpeed = 10.0f;
    [SerializeField] private float runSpeed = 20.0f;
    [SerializeField] SanityBar sanityBar;
    [SerializeField] GrogScript grogScript;
    [SerializeField] private AlertEmitter emitter;

    public InputAction playerControls;
    private Vector2 moveDirection = Vector2.zero;
    private float emitSoundInterval = 0.5f;
    private float emitSoundTimer = 0.0f;

    private float sanityTimer = 0.0f;
    private float sanityDuration = 10.0f;
    private bool abilityInUse = false;

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
        sanityBar.SetMaxSanity(sanityDuration);
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
            emitSoundTimer += Time.deltaTime;
            if (emitSoundTimer >= emitSoundInterval)
            {
                if (!abilityInUse)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        emitter.EmitAlert(transform.position, AlertType.FOOTSTEPS_RUN);
                        SoundManager.PlaySound(SoundType.PLAYER_FOOTSTEPS);
                        SoundManager.PlaySound(SoundType.PLAYER_FOOTSTEPS);
                    }
                    else
                    {
                        emitter.EmitAlert(transform.position, AlertType.FOOTSTEPS_WALK);
                        SoundManager.PlaySound(SoundType.PLAYER_FOOTSTEPS);

                    }
                }
                emitSoundTimer = 0.0f;

            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sanityTimer < sanityDuration)
            {
                abilityInUse = !abilityInUse;
                if (abilityInUse)
                {
                    gameObject.layer = LayerMask.NameToLayer("RaccoonInvisible");
                    emitter.EmitAlert(transform.position, AlertType.USED_ABILITY);
                    SoundManager.PlaySound(SoundType.ABILITY_USED);
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Raccoon");
                }

            }

        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (grogScript.RemoveGrogImage())
            {
                Instantiate(grogPrefab, transform.position, Quaternion.identity);
            }
        }

        if (abilityInUse)
        {
            drainSanity();
        }
    }

    void drainSanity()
    {
        if (sanityTimer >= sanityDuration)
        {
            Debug.Log("Sanity finished");
            abilityInUse = false;
            return;
        }
        sanityTimer += Time.deltaTime;
        sanityBar.SetSanity(sanityDuration - sanityTimer);
    }
}
