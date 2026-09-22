using UnityEngine;

public class PlatformerMovement : PlayerMovement
{
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float gravityScale = 3f;
    [SerializeField] float groundCheckDistance = 0.08f;
    [SerializeField] float minGroundNormalY = 0.5f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBufferTime = 0.1f;
    [SerializeField] float fallGravityMultiplier = 1.8f;
    [SerializeField] float lowJumpGravityMultiplier = 2.5f;
    [SerializeField] LayerMask groundMask = ~0;

    Collider2D bodyCollider;
    readonly ContactPoint2D[] contactPoints = new ContactPoint2D[8];
    readonly RaycastHit2D[] castHits = new RaycastHit2D[4];
    float coyoteCounter;
    float jumpBufferCounter;
    bool jumpHeld;

    protected override void Awake()
    {
        base.Awake();
        bodyCollider = GetComponent<Collider2D>();
    }

    protected override void ConfigureBody()
    {
        Body.gravityScale = gravityScale;
    }

    protected override void ReadModeInput()
    {
        jumpHeld = ReadJumpHeld();
        if (ReadJumpDown())
            jumpBufferCounter = jumpBufferTime;
    }

    protected virtual bool ReadJumpDown()
    {
        return Controls.Player.Jump.WasPressedThisFrame();
    }

    protected virtual bool ReadJumpHeld()
    {
        return Controls.Player.Jump.IsPressed();
    }

    protected override Vector2 ComputeDirection(Vector2 input)
    {
        return new Vector2(Mathf.Clamp(input.x, -1f, 1f), 0f);
    }

    protected override Vector2 ComputeVelocity(Vector2 direction)
    {
        float dt = Time.fixedDeltaTime;
        if (IsGrounded())
            coyoteCounter = coyoteTime;
        else
            coyoteCounter = Mathf.Max(0f, coyoteCounter - dt);

        float verticalSpeed = Body.velocity.y;
        if (coyoteCounter > 0f && jumpBufferCounter > 0f)
        {
            verticalSpeed = jumpSpeed;
            coyoteCounter = 0f;
            jumpBufferCounter = 0f;
        }
        else if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= dt;
        }

        ApplyGravityScale(verticalSpeed);
        return new Vector2(direction.x * MoveSpeed, verticalSpeed);
    }

    void ApplyGravityScale(float verticalSpeed)
    {
        if (verticalSpeed < -0.01f)
            Body.gravityScale = gravityScale * fallGravityMultiplier;
        else if (verticalSpeed > 0.01f && !jumpHeld)
            Body.gravityScale = gravityScale * lowJumpGravityMultiplier;
        else
            Body.gravityScale = gravityScale;
    }

    bool IsGrounded()
    {
        int contactCount = Body.GetContacts(contactPoints);
        for (int i = 0; i < contactCount; i++)
        {
            if (IsFloor(contactPoints[i].normal))
                return true;
        }

        if (bodyCollider == null)
            bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null)
            return false;

        var filter = new ContactFilter2D();
        filter.SetLayerMask(groundMask);
        filter.useTriggers = false;
        int hitCount = bodyCollider.Cast(Vector2.down, filter, castHits, groundCheckDistance);
        for (int i = 0; i < hitCount; i++)
        {
            if (IsFloor(castHits[i].normal))
                return true;
        }

        return false;
    }

    bool IsFloor(Vector2 normal)
    {
        // Edge colliders follow vertex winding, so a floor can report a downward normal.
        if (normal.y < 0f)
            normal = -normal;
        return normal.y >= minGroundNormalY;
    }
}
