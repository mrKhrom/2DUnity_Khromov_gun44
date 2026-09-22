using UnityEngine;

public class TopDownMovement : PlayerMovement
{
    protected override void ConfigureBody()
    {
        Body.gravityScale = 0f;
    }

    protected override Vector2 ComputeDirection(Vector2 input)
    {
        return MovementMath.ClampToUnit(input);
    }

    protected override Vector2 ComputeVelocity(Vector2 direction)
    {
        return direction * MoveSpeed;
    }
}
