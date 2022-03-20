using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct EntityPrefabComponent : IComponentData
{
    public Entity PrefabEntity;
    public int NumberToSpawn;
    public float3 SpawnLocation;
    public bool RandomSpawnLocation;
}
