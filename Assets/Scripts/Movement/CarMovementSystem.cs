using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

public class CarMovementSystem : ComponentSystem
{
    /// <summary>
    /// Tested with a Job + Burst Compiler and it was actually slower.
    /// </summary>
    protected override void OnUpdate()
    {
        Entities.ForEach((ref CarMovementComponent carMoveComp, ref PathfindingComponent pathFindingComp,
            ref Translation translationRef, ref Rotation rotationRef) =>
        {
            // Car is in the air/ upside-down.
            if (translationRef.Value.y > 1.5f) return;

            // Find destination.
            float3 destination = pathFindingComp.TargetPosition;
            float3 direction = destination - translationRef.Value;

            // Rotate towards the destination.
            if (direction.x != 0 || direction.z != 0)
            {
                rotationRef.Value = MathamaticsUtils.Lerp(rotationRef.Value,
                quaternion.LookRotation(direction, new float3(0, 1, 0)),
                carMoveComp.TurnSpeed * Time.DeltaTime);
            }

            // Interpolate car to its destination.
            translationRef.Value = MathamaticsUtils.MoveTowards(translationRef.Value, destination, carMoveComp.MoveSpeed * Time.DeltaTime);

        });

    }

}
