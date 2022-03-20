using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Waypoint StartingLine;
    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Waypoint node dynamic buffer creation.
        EntityArchetype nodeArchetype = entityManager.CreateArchetype(typeof(Tag_NodeManager));
        Entity nodeEntity = entityManager.CreateEntity(nodeArchetype);
        DynamicBuffer<NodeBufferElement> nodes = entityManager.AddBuffer<NodeBufferElement>(nodeEntity);
        var startingNode = MathamaticsUtils.GetNode(StartingLine, 0);
        nodes.Add(startingNode);

        // Set up node tree.
        Waypoint currentWaypoint = StartingLine;
        while (currentWaypoint.NextWaypoint != null &&
            currentWaypoint.NextWaypoint != StartingLine)
        {
            nodes.Add(MathamaticsUtils.GetNode(currentWaypoint.NextWaypoint, nodes.Length));
            currentWaypoint = currentWaypoint.NextWaypoint;
        }
    }
}
