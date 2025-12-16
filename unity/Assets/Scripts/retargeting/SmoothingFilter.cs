using UnityEngine;

public class SmoothingFilter
{
    float smoothSpeed = 15f; // increased for smoother hand tracking

    public Quaternion Smooth(Quaternion current, Quaternion target)
    {
        if (current == Quaternion.identity)
            return target;

        return Quaternion.Slerp(
            current,
            target,
            Time.deltaTime * smoothSpeed
        );
    }
}
