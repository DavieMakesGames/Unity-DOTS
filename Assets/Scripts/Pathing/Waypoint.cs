using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Game object representation of a Pathfinding node.
public class Waypoint: MonoBehaviour
{
    public Waypoint PreviousWaypoint;
    public Waypoint NextWaypoint;
    public float Width = 10;
}
