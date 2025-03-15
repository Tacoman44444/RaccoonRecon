using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    public float viewRadius = 5.0f;
    [Range(0, 360)] public float viewAngle = 90.0f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public bool CanSeePlayer { get; private set; } = false;
    public event Action<Transform> OnPlayerSensed;

    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Raccoon").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckPlayerInVision())
        {
            OnPlayerSensed?.Invoke(player.transform);
        }
    }

    private bool CheckPlayerInVision()
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < viewRadius)
        {
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);
            if (angleToPlayer < viewAngle / 2)
            {
                
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
                if (hit.collider == null)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
