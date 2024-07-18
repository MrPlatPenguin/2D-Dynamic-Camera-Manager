using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ECharacterState
    {
        Idle,
        Jumping,
        Falling
    }

    [field: SerializeField] public ECharacterState CurrentState { get; private set; } = ECharacterState.Falling;
    public Vector2 velocity;
    [SerializeField] BoxCollider2D collisionBox;
    [SerializeField] MovementData movementData;

    Rigidbody2D rb;

    //Jumping
    float jumpTime = 0f;
    bool leftPressed;
    bool rightPressed;
    bool jumpPressed;

    private void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        leftPressed = Input.GetKey(KeyCode.A);
        rightPressed = Input.GetKey(KeyCode.D);
        jumpPressed = Input.GetKey(KeyCode.Space);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = rb.velocity;

        HandleState();

        rb.velocity = velocity;
    }

    void HandleState()
    {
        switch (CurrentState)
        {
            case ECharacterState.Idle:
                IdleUpdate();
                break;
            case ECharacterState.Jumping:
                JumpUpdate();
                break;
            case ECharacterState.Falling:
                FallingUpdate();
                break;
            default:
                break;
        }
    }

    bool GetGround(out RaycastHit2D hit)
    {
        hit = new RaycastHit2D();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, collisionBox.size, 0, Vector3.down, movementData.groundingDistance, movementData.groundLayer);
        foreach (RaycastHit2D hita in hits)
        {
            if (hita.collider.gameObject != gameObject)
            {
                hit = hita;
                break;
            }
        }
        return hit;
    }

    bool GetGround()
    {
        return GetGround(out RaycastHit2D hit);
    }

    void FallingEnter()
    {
        CurrentState = ECharacterState.Falling;
    }

    void FallingUpdate()
    {
        velocity.y -= movementData.fallAcceleration * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -movementData.maxFallSpeed, Mathf.Infinity);

        HandleMove();
        if (velocity.y < 0 && GetGround())
            IdleEnter();
    }

    void JumpEnter()
    {
        CurrentState = ECharacterState.Jumping;
        velocity.y = movementData.verticalJumpForce;
        jumpTime = 0f;
    }

    void JumpUpdate()
    {
        HandleMove();
        jumpTime += Time.deltaTime;
        if (jumpTime >= movementData.maxJumpTime)
            FallingEnter();
    }

    void IdleEnter()
    {
        CurrentState = ECharacterState.Idle;
        velocity.y = -2f;
    }

    void IdleUpdate()
    {
        HandleMove();

        if (jumpPressed)
            JumpEnter();
        if (!GetGround())
            FallingEnter();
    }

    void HandleMove()
    {
        float targetSpeed;
        if (leftPressed)
            targetSpeed = -movementData.MaxSpeed;
        else if (rightPressed)
            targetSpeed = movementData.MaxSpeed;
        else
            targetSpeed = 0f;

        bool inputDirSameAsVel = (targetSpeed > 0 && velocity.x > 0) || (targetSpeed < 0 && velocity.x < 0);
        bool fasterThanMoveSpeed = Mathf.Abs(velocity.x) > Mathf.Abs(targetSpeed);
        //Deceleration
        if (targetSpeed == 0 || (inputDirSameAsVel && fasterThanMoveSpeed))
        {
            var deceleration = movementData.Deceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        //Acceleration
        else
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, movementData.Acceleration * Time.fixedDeltaTime);
    }
}
