using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HelperFunctions;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyVisionCone : MonoBehaviour
{
    public float viewRadius = 4.0f;
    [Range(0, 360)] public float viewAngle = 120.0f;

    public LayerMask obstacleMask;
    public LayerMask wallMask;

    public event Action<Transform> playerInVision;
    public event Action playerUnobstructed;

    [SerializeField] private Transform fovPrefab;
    private FieldOfView fieldOfView;

    private Transform player;

    void Start()
    {
        fieldOfView = Instantiate(fovPrefab, null).GetComponent<FieldOfView>();
        player = GameObject.FindGameObjectWithTag("Raccoon").transform;
    }

    void Update()
    {
        SetVisionConeParameters();
        if (CheckPlayerInVision(obstacleMask))
        {
            playerInVision?.Invoke(player.transform);
        }
        else if (CheckPlayerInVision(wallMask))
        {
            playerUnobstructed?.Invoke();
        }
    }

    private void SetVisionConeParameters()
    {
        fieldOfView.SetOrigin(transform.position);
        float angle = (transform.eulerAngles.z + 45.0f) * Mathf.Deg2Rad; // Convert to radians
        Vector2 lookDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        fieldOfView.SetDirection(MyMathUtils.GetVectorFromAngle(transform.eulerAngles.z + 45.0f));
        fieldOfView.SetViewDistance(viewRadius);
        fieldOfView.SetFOV(viewAngle);
    }

    private bool CheckPlayerInVision(LayerMask obstacleMask)
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < viewRadius)
        {
            float angleToPlayer = Vector2.Angle(MyMathUtils.GetVectorFromAngle(transform.eulerAngles.z), directionToPlayer);
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
