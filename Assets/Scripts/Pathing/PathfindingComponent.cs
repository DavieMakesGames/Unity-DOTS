using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PathfindingComponent : IComponentData
{
    public NodeBufferElement TargetNode;
    public float3 TargetPosition;
    public bool TargetBackwards;
    public float TargetWidthOffset;
}
