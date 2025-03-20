using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HelperFunctions;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyVisionCone : MonoBehaviour
{
    public float viewRadius = 5.0f;
    [Range(0, 360)] public float viewAngle = 90.0f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public event Action<Transform> OnPlayerSensed;
    public event Action OnPlayerSpotted;

    [SerializeField] private Transform fovPrefab;
    private FieldOfView fieldOfView;

    private Transform player;
    private float spotTimer = 2.0f;
    private float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        fieldOfView = Instantiate(fovPrefab, null).GetComponent<FieldOfView>();
        player = GameObject.FindGameObjectWithTag("Raccoon").transform;
    }

    // Update is called once per frame
    void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        float angle = (transform.eulerAngles.z + 45.0f) * Mathf.Deg2Rad; // Convert to radians
        Vector2 lookDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        fieldOfView.SetDirection(MyMathUtils.GetVectorFromAngle(transform.eulerAngles.z + 45.0f));
        fieldOfView.SetViewDistance(viewRadius);

        if (CheckPlayerInVision())
        {
            timer += Time.deltaTime;
            OnPlayerSensed?.Invoke(player.transform);
            if (timer > spotTimer)
            {
                OnPlayerSpotted?.Invoke();
            }
        } 
        else if (!CheckPlayerInVision())
        {
            timer = 0.0f;
        }
    }

    private bool CheckPlayerInVision()
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
