using HelperFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSprayVisionCone : MonoBehaviour
{
    public float viewRadius = 5.0f;
    [Range(0, 360)] public float viewAngle = 90.0f;

    [SerializeField] private Transform fovPrefab;
    private FieldOfView fieldOfView;
    private EnemyAI[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        enemies = FindObjectsOfType<EnemyAI>();
        fieldOfView = Instantiate(fovPrefab, null).GetComponent<FieldOfView>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
        mousePosition.z = 0;
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(MyMathUtils.RotateVectorByAngle(mousePosition - transform.position, viewAngle / 2));
        fieldOfView.SetViewDistance(viewRadius);
        if (Input.GetMouseButtonDown(0))
        {
            foreach (EnemyAI enemyInVision in CheckEnemiesInVision(mousePosition))
            {
                Debug.Log("enemyInVision is not empty");
                enemyInVision.Sleep();
            }
        }
        
    }

    private List<EnemyAI> CheckEnemiesInVision(Vector3 mousePosition)
    {
        List<EnemyAI> enemiesInVision = new List<EnemyAI>();
        float angle = MyMathUtils.GetAngleFromVectorFloat(mousePosition - transform.position);
        foreach (EnemyAI enemy in enemies)
        {
            Debug.Log("Enemy list is not empty");
            if (Vector3.Distance(transform.position, enemy.transform.position) < viewRadius)
            {
                float angleToEnemy = MyMathUtils.GetAngleFromVectorFloat(enemy.transform.position - transform.position);
                Debug.Log("ANGLE TO ENEMY: " +  angleToEnemy);
                Debug.Log("MOUSE DIRECTION ANGLE: " + angle);
                Debug.Log("ANGLE: " + Math.Abs(angleToEnemy - angle));
                if (Math.Abs(angleToEnemy - angle) < viewAngle / 2)
                {
                    enemiesInVision.Add(enemy);
                }
            }
        }
        return enemiesInVision;
    }
}
