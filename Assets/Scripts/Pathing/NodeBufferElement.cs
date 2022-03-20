using Unity.Entities;
using Unity.Mathematics;

public struct NodeBufferElement : IBufferElementData
{
    public float3 Position;
    public float3 Forward;
    public float3 Right;
    public float Width;
    public int Index;
}
