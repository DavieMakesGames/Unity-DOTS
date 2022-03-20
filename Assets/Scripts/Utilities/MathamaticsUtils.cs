using System;
using Unity.Mathematics;

/// <summary>
/// Utility class for functions not currently found in Unity.Mathematics.
/// </summary>
public static class MathamaticsUtils
{
    /// <summary>
    /// Smoothly move from one float3 to another with no slowdown or smoothness.
    /// </summary>
    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        float deltaX = target.x - current.x;
        float deltaY = target.y - current.y;
        float deltaZ = target.z - current.z;
        float sqdist = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        if (sqdist == 0 || sqdist <= maxDistanceDelta * maxDistanceDelta)
            return target;
        var dist = (float)Math.Sqrt(sqdist);
        return new float3(current.x + deltaX / dist * maxDistanceDelta,
            current.y + deltaY / dist * maxDistanceDelta,
            current.z + deltaZ / dist * maxDistanceDelta);
    }
    /// <summary>
    /// Linearally Interpolate from one quaternion value to another.
    /// </summary>
    public static quaternion Lerp(quaternion q1, quaternion q2, float t)
    {
        float dt = math.dot(q1, q2);
        if (dt < 0.0f)
        {
            q2.value = -q2.value;
        }

        return math.normalize(math.quaternion(math.lerp(q1.value, q2.value, t)));
    }
    /// <summary>
    /// Convert Waypoint objects into NodeBufferElement.
    /// </summary>
    public static NodeBufferElement GetNode(Waypoint waypoint, int index)
    {
        float3 position = new float3(
                waypoint.transform.position.x,
                waypoint.transform.position.y,
                waypoint.transform.position.z);
        float3 forward = new float3(
                waypoint.transform.forward.x,
                waypoint.transform.forward.y,
                waypoint.transform.forward.z);
        float3 right = new float3(
                waypoint.transform.right.x,
                waypoint.transform.right.y,
                waypoint.transform.right.z);
        return new NodeBufferElement { Position = position, Right = right, Forward = forward, Width = waypoint.Width, Index = index };
    }
}