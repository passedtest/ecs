using ECS;
using UnityEngine;

public sealed class MovementSystem : ExtendedSystem
{
    readonly EntityQuery m_EntityQueryTest;

    public MovementSystem() 
    {
        m_EntityQueryTest = 
            new EntityQueryBuilder()
                .All()
                .Of<BehaviourComponent>()
                .Of<MoveComponent>()
                .Of<DestinationComponent>()
                .ToQuery();
    }

    protected override void OnExecuteUpdate(in ECSWorld world, in float deltaTime)
    {
        base.OnExecuteUpdate(world, deltaTime);

        Single<WorldTimeComponent>.TryGet(world.UID, out var timeComponent);

        var behaviourComponents = world.GetComponentsWithEntityReadOnly<BehaviourComponent>();
        var movementComponents = world.GetComponentsWithEntity<MoveComponent>();
        var destinationComponents = world.GetComponentsWithEntity<DestinationComponent>();

        foreach (var targetEntity in m_EntityQueryTest.ForWorld(world.UID))
        {
            if (!behaviourComponents.TryGetComponent(targetEntity, out var behaviourComponent))
                continue;

            if (!movementComponents.TryGetComponent(targetEntity, out var movementComponent))
                continue;

            if (!destinationComponents.TryGetComponent(targetEntity, out var destinationComponent))
                continue;

            var directionToDestination = destinationComponent.Destination - movementComponent.Position;
            movementComponent.Orientation = directionToDestination.normalized;
            var desiredMovement = movementComponent.Orientation * deltaTime * behaviourComponent.Speed;
            movementComponent.Position += desiredMovement;

            //Update destination
            if (desiredMovement.sqrMagnitude >= directionToDestination.sqrMagnitude || destinationComponent.NextDestinationUpdateTime < timeComponent.Time)
            {
                destinationComponent.NextDestinationUpdateTime = timeComponent.Time + behaviourComponent.DestinationChangePeriod;
                destinationComponent.Destination = UnityEngine.Random.insideUnitSphere * 30f;
            }

            //Update component
            movementComponents.TrySetComponent(targetEntity, movementComponent);
            destinationComponents.TrySetComponent(targetEntity, destinationComponent);
        }
    }
}

[System.Serializable]
public struct BehaviourComponent : IComponent
{
    public AgentTeam Team;
    public float Speed;
    public float DestinationChangePeriod;

    void IComponent.Register(in int world, in int entity) => ECS.Core.ComponentMap<BehaviourComponent>.TryAddOrSet(world, entity, this);
}

[System.Serializable]
public struct DestinationComponent : IComponent
{
    public Vector3 Destination;
    public float NextDestinationUpdateTime;

    void IComponent.Register(in int world, in int entity) => ECS.Core.ComponentMap<DestinationComponent>.TryAddOrSet(world, entity, this);
}

[System.Serializable]
public struct MoveComponent : IComponent
{
    public Vector3 Position;
    public Vector3 Orientation;

    void IComponent.Register(in int world, in int entity) => ECS.Core.ComponentMap<MoveComponent>.TryAddOrSet(world, entity, this);
}

public enum AgentTeam
{
    Red, Green, Blue,
}


