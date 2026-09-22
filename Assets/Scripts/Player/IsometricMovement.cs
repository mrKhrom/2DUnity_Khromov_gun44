using UnityEngine;

public class IsometricMovement : PlayerMovement
{
    [SerializeField] float verticalScale = 0.5f;

    protected override void Awake()
    {
        Grid grid = FindObjectOfType<Grid>();
        if (grid != null &&
            grid.cellLayout == GridLayout.CellLayout.Isometric &&
            Mathf.Abs(grid.cellSize.x) > 0.0001f)
        {
            verticalScale = Mathf.Abs(grid.cellSize.y / grid.cellSize.x);
        }

        base.Awake();
    }

    protected override void ConfigureBody()
    {
        Body.gravityScale = 0f;
    }

    protected override Vector2 ComputeDirection(Vector2 input)
    {
        return MovementMath.ToIsometric(input, verticalScale);
    }

    protected override Vector2 ComputeVelocity(Vector2 direction)
    {
        return direction * MoveSpeed;
    }
}
