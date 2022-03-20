using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PathfindingNode : IComponentData
{
    public float3 Position;
    public int CurrentIndex;
    public int PreviousIndex;
}
