using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using System.Collections.Generic;

public class PathfindingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Locate the node dynamic buffer.
        //If this is placed outside of the current scope an exception will be thrown due to deallocation.
        Entity nodeManager = Entity.Null;
        Entities.WithAll<Tag_NodeManager>().ForEach((Entity nodeManagerTemp) =>
        {
                nodeManager = nodeManagerTemp;
        });
        BufferFromEntity<NodeBufferElement> nodeBuffers = GetBufferFromEntity<NodeBufferElement>();
        DynamicBuffer<NodeBufferElement> bufferFromEntity = nodeBuffers[nodeManager];

        // Create non-native collection to fill nativeArrays for the job.
        List<PathFindingStruct> pathFindingStructs = new List<PathFindingStruct>();
        Entities.ForEach((ref PathfindingComponent pathfindingCompRef, ref Translation translationRef) =>
        {
            PathFindingStruct pfs = new PathFindingStruct
            {
                TargetNode = pathfindingCompRef.TargetNode,
                TargetPosition = pathfindingCompRef.TargetPosition,
                TargetBackwards = pathfindingCompRef.TargetBackwards,
                TargetWidthOffset = pathfindingCompRef.TargetWidthOffset,
                CurrentPosition = translationRef.Value
            };
            pathFindingStructs.Add(pfs);
        });

        // Create lists to pass in pathfinding information to job.
        NativeArray<NodeBufferElement> targetNodes = new NativeArray<NodeBufferElement>(pathFindingStructs.Count, Allocator.TempJob);
        NativeArray<float3> targetPositions = new NativeArray<float3>(pathFindingStructs.Count, Allocator.TempJob);
        NativeArray<bool> targetBackwards = new NativeArray<bool>(pathFindingStructs.Count, Allocator.TempJob);
        NativeArray<float> targetWidthOffsets = new NativeArray<float>(pathFindingStructs.Count, Allocator.TempJob);
        NativeArray<float3> currentPositions = new NativeArray<float3>(pathFindingStructs.Count, Allocator.TempJob);
        for (int i = 0; i < pathFindingStructs.Count; i++)
        {
            targetNodes[i] = pathFindingStructs[i].TargetNode;
            targetPositions[i] = pathFindingStructs[i].TargetPosition;
            targetBackwards[i] = pathFindingStructs[i].TargetBackwards;
            targetWidthOffsets[i] = pathFindingStructs[i].TargetWidthOffset;
            currentPositions[i] = pathFindingStructs[i].CurrentPosition;
        }

        // Create job and pass in parameters.
        PathfindingParallelJob parallelJob = new PathfindingParallelJob
        {
            TargetNodes = targetNodes,
            TargetPositions = targetPositions,
            TargetBackwards = targetBackwards,
            TargetWidthOffsets = targetWidthOffsets,
            CurrentPositions = currentPositions,
            BufferFromEntity = bufferFromEntity,
        };

        // Schedule job.
        JobHandle parallelJobHandle = parallelJob.Schedule(pathFindingStructs.Count, 1000);
        parallelJobHandle.Complete();

        // Once the job has completed the calculations, set the updated values on each entity.
        int j = 0;
        Entities.ForEach((ref PathfindingComponent pathfindingCompRef, ref Translation translationRef) =>
        {
            pathfindingCompRef.TargetNode = targetNodes[j];
            pathfindingCompRef.TargetPosition = targetPositions[j];
            j++;
        });

        // Dispose of the native collections.
        targetNodes.Dispose();
        targetPositions.Dispose();
        targetBackwards.Dispose();
        targetWidthOffsets.Dispose();
        currentPositions.Dispose();
    }

    // Item used to set up pathfinding job.
    public struct PathFindingStruct
    {
        public NodeBufferElement TargetNode;
        public float3 TargetPosition;
        public bool TargetBackwards;
        public float TargetWidthOffset;
        public float3 CurrentPosition;
    }

    [BurstCompile]
    public struct PathfindingParallelJob : IJobParallelFor
    {
        public NativeArray<NodeBufferElement> TargetNodes;
        public NativeArray<float3> TargetPositions;
        public NativeArray<bool> TargetBackwards;
        public NativeArray<float> TargetWidthOffsets;
        public NativeArray<float3> CurrentPositions;

        [NativeDisableParallelForRestriction]
        public DynamicBuffer<NodeBufferElement> BufferFromEntity;

        public void Execute(int index)
        {
            // Set destination to a random range using the width of the target node.
            float3 min = TargetNodes[index].Position - TargetNodes[index].Right * TargetNodes[index].Width / 2;
            float3 max = TargetNodes[index].Position + TargetNodes[index].Right * TargetNodes[index].Width / 2;
            TargetPositions[index] = math.lerp(min, max, TargetWidthOffsets[index]);

            // Change node to the next target if the car has reached its destination.
            if (math.distance(CurrentPositions[index], TargetPositions[index]) < 1)
            {
                if (TargetBackwards[index])
                {
                    // Change the target node to the previous node.
                    int prevIndex = TargetNodes[index].Index - 1;
                    if (prevIndex < 0) TargetNodes[index] = BufferFromEntity[BufferFromEntity.Length-1];
                    else TargetNodes[index] = BufferFromEntity[prevIndex];
                }
                else
                {
                    // Change the target node to the previous node.
                    int nextIndex = TargetNodes[index].Index + 1;
                    if (BufferFromEntity.Length > nextIndex) TargetNodes[index] = BufferFromEntity[nextIndex];
                    else TargetNodes[index] = BufferFromEntity[0];
                }
                
            }
        }
    }
}
