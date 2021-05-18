using ECS;

public sealed class WorldTimeSystem : ExtendedSystem
{
    protected override void OnExecuteUpdate(in ECSWorld world, in float deltaTime)
    {
        base.OnExecuteUpdate(world, deltaTime);

        var localDeltaTime = deltaTime;
        Single<WorldTimeComponent>.Change(world.UID, (ref WorldTimeComponent timeComponent) => 
        {
            timeComponent.Time += localDeltaTime;
            timeComponent.TickCount++;
        });
    }

    protected override void OnExecuteFixedUpdate(in ECSWorld world, in float deltaTime)
    {
        base.OnExecuteFixedUpdate(world, deltaTime);

        var localDeltaTime = deltaTime;
        Single<WorldFixedTimeComponent>.Change(world.UID, (ref WorldFixedTimeComponent timeComponent) =>
        {
            timeComponent.FixedTime += localDeltaTime;
            timeComponent.FixedTickCount++;
        });
    }
}

[System.Serializable]
public struct WorldTimeComponent : IComponent
{
    public float Time;
    public int TickCount;

    void IComponent.Register(int world, int entity) => ECS.Core.ComponentMap<WorldTimeComponent>.TryAddOrSet(world, entity, this);
}

[System.Serializable]
public struct WorldFixedTimeComponent : IComponent
{
    public float FixedTime;
    public int FixedTickCount;

    void IComponent.Register(int world, int entity) => ECS.Core.ComponentMap<WorldFixedTimeComponent>.TryAddOrSet(world, entity, this);
}
