using UnityEngine;

public static class MovementMath
{
    public static Vector2 ClampToUnit(Vector2 input)
    {
        if (input.sqrMagnitude > 1f)
            return input.normalized;
        return input;
    }

    // Unity isometric cell: +X maps to (1, verticalScale), +Y maps to (-1, verticalScale).
    public static Vector2 ToIsometric(Vector2 input, float verticalScale)
    {
        Vector2 axisX = new Vector2(1f, verticalScale).normalized;
        Vector2 axisY = new Vector2(-1f, verticalScale).normalized;
        Vector2 world = axisX * input.x + axisY * input.y;
        float scale = Mathf.Clamp01(input.magnitude);
        if (scale <= 0f || world.sqrMagnitude < 0.000001f)
            return Vector2.zero;
        return world.normalized * scale;
    }
}
