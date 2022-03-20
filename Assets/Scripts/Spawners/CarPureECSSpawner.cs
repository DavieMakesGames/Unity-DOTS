using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;

/// <summary>
/// This is an example of pure ECS implementation.
/// No gameObjects involved.
/// </summary>
public class CarPureECSSpawner : MonoBehaviour
{
    public int NumberToSpawn = 1;
    public Mesh Mesh;
    public Material Material;
    public bool RandomSpawnLocation = true;

    Unity.Mathematics.Random _random = new Unity.Mathematics.Random(345253);
    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Creating the car entity.
        EntityArchetype carArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(CarMovementComponent),
            typeof(PathfindingComponent))
            ;
        NativeArray<Entity> carArray = new NativeArray<Entity>(NumberToSpawn, Allocator.Temp);
        entityManager.CreateEntity(carArchetype, carArray);
        
        var colors = new List<Color>();
        foreach (Entity entity in carArray)
        {
            // Rendering
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = Mesh,
                material = Material,
            });

            // Movement
            entityManager.SetComponentData(entity, new CarMovementComponent
            {
                MoveSpeed = UnityEngine.Random.Range(10,100),
                TurnSpeed = 10,
            });

            // Waypoint node dynamic buffer.
            NodeBufferElement startingNode;
            if (RandomSpawnLocation)
            {
                Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
                int index = _random.NextInt(0, waypoints.Length - 1);
                Debug.Log(index);
                startingNode = MathamaticsUtils.GetNode(waypoints[index], index);
            }
            else
            {
                WaypointManager waypointManager = FindObjectOfType<WaypointManager>();
                startingNode = MathamaticsUtils.GetNode(waypointManager.StartingLine, 0);
            }

            // Spawn Location
            entityManager.SetComponentData(entity, new Translation
            {
                Value = startingNode.Position
            });

            // Pathfinding
            entityManager.SetComponentData(entity, new PathfindingComponent
            {
                TargetNode = startingNode,
                TargetBackwards = _random.NextBool(),
                TargetWidthOffset = _random.NextFloat(0, 1)
            });

        }
        carArray.Dispose();
    }
}
