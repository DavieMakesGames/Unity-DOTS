using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// This is an example of a hybrid ECS implemtation. 
/// A gameObject is taken and converted into an Entity.
/// </summary>
public class CarHybridSpawner : ComponentSystem
{
    Unity.Mathematics.Random _random = new Unity.Mathematics.Random(345253);
    protected override void OnStartRunning()
    {
        // Find all EntityPrefabComponents in the scene and use their parameters to spawn entities.
        Entities.ForEach((ref EntityPrefabComponent entityPrefabComp) =>
        {
            // Fill Entity array
            NativeArray<Entity> entArray = new NativeArray<Entity>(entityPrefabComp.NumberToSpawn, Allocator.Temp);
            EntityManager.Instantiate(entityPrefabComp.PrefabEntity, entArray);

            // Set the parameters for every entity spawned.
            foreach (Entity entity in entArray)
            {
                // Movement
                EntityManager.SetComponentData(entity, new CarMovementComponent
                {
                    MoveSpeed = UnityEngine.Random.Range(3, 50),
                    TurnSpeed = 10,
                });

                // Locate the node dynamic buffer. 
                //If this is placed outside of the current scope an exception will be thrown due to deallocation.
                Entity nodeManager = Entity.Null;
                Entities.WithAll<Tag_NodeManager>().ForEach((Entity nodeManagerTemp) =>
                {
                    nodeManager = nodeManagerTemp;
                });
                BufferFromEntity<NodeBufferElement> nodeBuffers = GetBufferFromEntity<NodeBufferElement>();
                DynamicBuffer<NodeBufferElement> bufferFromEntity = nodeBuffers[nodeManager];

                // Pathfinding
                NodeBufferElement targetNode;
                if (entityPrefabComp.RandomSpawnLocation) targetNode = bufferFromEntity[_random.NextInt(0, bufferFromEntity.Length - 1)];
                else targetNode = bufferFromEntity[0];

                EntityManager.SetComponentData(entity, new PathfindingComponent
                {
                    TargetNode = targetNode,
                    TargetBackwards = _random.NextBool(),
                    TargetWidthOffset = _random.NextFloat(0, 1)
                });

                // Spawn location
                EntityManager.SetComponentData(entity, new Translation
                {
                    Value = targetNode.Position
                });
            }
        });
    }
    protected override void OnUpdate()
    {
    }
}
