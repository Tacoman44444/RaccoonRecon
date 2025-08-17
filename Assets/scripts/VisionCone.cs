using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private VisionConeShape coneShape = new VisionConeShape();

    public bool EntityInSight(Vector3 entityPosition, LayerMask obstructionMask)
    {
        Vector2 dirToPlayer = entityPosition - transform.position;
        float distanceToPlayer = dirToPlayer.magnitude;

        if (distanceToPlayer > coneShape.coneRadius) return false;

        float angleToPlayer = Vector2.Angle(transform.right, dirToPlayer.normalized);
        if (angleToPlayer > coneShape.coneAngle / 2f) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer.normalized, distanceToPlayer, obstructionMask);
        return hit.collider == null;
    }

    private void OnDrawGizmos()
    {
        if (coneShape != null)
        {
            coneShape.DrawGizmos(transform.position, transform.right);
        }
    }

}

[System.Serializable]
public class VisionConeShape
{
    public float coneAngle = 60.0f;
    public float coneRadius = 7.0f;
    int segments = 30;
    float edgeFadePercent = 0.1f;

    public void DrawGizmos(Vector3 origin, Vector3 forward)
    {
        Color baseColor = new Color(1f, 1f, 0f, 0.2f);  
        Color edgeColor = new Color(1f, 1f, 0f, 0f);

        float halfAngle = coneAngle * 0.5f;
        Quaternion leftRot = Quaternion.Euler(0, 0, -halfAngle);
        Quaternion rightRot = Quaternion.Euler(0, 0, halfAngle);

        Vector3 leftDir = leftRot * forward.normalized;
        Vector3 rightDir = rightRot * forward.normalized;

        float angleIncrement = coneAngle / segments;

        Vector3 prevPoint = origin + leftDir * coneRadius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + angleIncrement * i;
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Vector3 dir = rot * forward.normalized;
            Vector3 nextPoint = origin + dir * coneRadius;

            Gizmos.color = baseColor;
            Gizmos.DrawLine(origin, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }

        // Optional: Draw soft edge indication
        float fadeRadius = coneRadius * edgeFadePercent;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireSphere(origin + forward.normalized * coneRadius, fadeRadius);
    }

}