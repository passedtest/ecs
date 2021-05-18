using ECS;
using UnityEngine;

public sealed class VIewMovementSystem : ExtendedSystem
{
    readonly EntityQuery m_EntityQueryTest;
      
    public VIewMovementSystem()
    {
        m_EntityQueryTest = 
            new EntityQueryBuilder()
                .All()
                .Of<MoveComponent>()
                .Of<ViewComponent>()
                .ToQuery();
    }

    protected override void OnExecuteUpdate(in ECSWorld world, in float deltaTime)
    {
        base.OnExecuteUpdate(world, deltaTime);

        var movmentComponents = world.GetComponentsWithEntityReadOnly<MoveComponent>();
        var viewComponents = world.GetComponentsWithEntity<ViewComponent>();

        foreach (var targetEntity in m_EntityQueryTest.ForWorld(world.UID))
        {
            if (!movmentComponents.TryGetComponent(targetEntity, out MoveComponent movementComponent))
                continue;

            if (!viewComponents.TryGetComponent(targetEntity, out ViewComponent viewComponent))
                continue;

            if (!ECS.Core.GameObjectByRef.TryGetObject(world.UID, viewComponent.ObjectReference, out var viewGameObject))
                continue;

            var viewTransform = viewGameObject.transform;

            viewTransform.localPosition = movementComponent.Position;
            viewTransform.localRotation = Quaternion.LookRotation(movementComponent.Orientation);
        }
    }
}



