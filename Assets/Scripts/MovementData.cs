using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Movement Data", menuName = "Movement Data")]
public class MovementData : ScriptableObject
{
    public float groundingDistance = 1f;
    public LayerMask groundLayer;
    public float fallAcceleration = 10f;
    public float maxFallSpeed = 10f;
    public float Acceleration = 10f;
    public float MaxSpeed = 15f;
    public float Deceleration = 1f;
    public float verticalJumpForce = 10f;
    public float horizontalJumpForce = 10f;
    public float maxJumpTime = 0.5f;
}
