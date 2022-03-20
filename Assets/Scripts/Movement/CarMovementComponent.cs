using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarMovementComponent : IComponentData
{
    public float MoveSpeed;
    public float TurnSpeed;
}
