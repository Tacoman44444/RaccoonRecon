using System;
using UnityEngine;

public enum AlertType
{
    FOOTSTEPS_WALK,
    FOOTSTEPS_RUN,
    BROKEN_BONES,
    USED_ABILITY,
}

public class AlertEmitter : MonoBehaviour
{
    public event Action<Vector2, AlertType> OnAlertEmitted;

    public void EmitAlert(Vector2 position, AlertType type)
    {
        OnAlertEmitted?.Invoke(position, type);
    }
}