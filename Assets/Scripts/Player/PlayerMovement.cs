using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public abstract class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;

    PlayerControls controls;

    protected Rigidbody2D Body { get; private set; }
    protected SpriteRenderer Sprite { get; private set; }
    protected Vector2 MoveInput { get; private set; }
    protected float MoveSpeed => moveSpeed;
    protected PlayerControls Controls => controls;

    protected virtual void Awake()
    {
        CacheComponents();
        Body.freezeRotation = true;
        Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Body.interpolation = RigidbodyInterpolation2D.Interpolate;
        ConfigureBody();
    }

    protected virtual void OnEnable()
    {
        CacheComponents();
        controls.Enable();
    }

    void CacheComponents()
    {
        if (controls == null)
            controls = new PlayerControls();
        if (Body == null)
            Body = GetComponent<Rigidbody2D>();
        if (Sprite == null)
            Sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnDisable()
    {
        if (controls != null)
            controls.Disable();
    }

    protected virtual void OnDestroy()
    {
        if (controls == null)
            return;
        controls.Dispose();
        controls = null;
    }

    protected virtual void ConfigureBody()
    {
    }

    protected virtual void Update()
    {
        MoveInput = ReadMoveInput();
        ReadModeInput();
    }

    protected virtual void FixedUpdate()
    {
        Vector2 direction = ComputeDirection(MoveInput);
        Body.velocity = ComputeVelocity(direction);
        ApplyFacing(direction);
    }

    protected virtual Vector2 ReadMoveInput()
    {
        return controls.Player.Move.ReadValue<Vector2>();
    }

    protected virtual void ReadModeInput()
    {
    }

    protected abstract Vector2 ComputeDirection(Vector2 input);

    protected abstract Vector2 ComputeVelocity(Vector2 direction);

    protected void ApplyFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
            Sprite.flipX = direction.x < 0f;
    }
}
