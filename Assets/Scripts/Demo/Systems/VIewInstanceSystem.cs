using ECS;

public sealed class VIewInstanceSystem : ExtendedSystem
{
    readonly EntitySetup[] m_EntitySetup;

    readonly EntityQuery m_EntityQuery;

    public VIewInstanceSystem(EntitySetup[] setup)
    {
        m_EntitySetup = setup;

        m_EntityQuery =
            new EntityQueryBuilder()
                .All()
                .Of<BehaviourComponent>()
                .ToQuery();
    }

    protected override void OnExecuteUpdate(in ECSWorld world, in float deltaTime)
    {
        base.OnExecuteUpdate(world, deltaTime);

        var behaviourComponents = world.GetComponentsWithEntityReadOnly<BehaviourComponent>();
        var viewComponents = world.GetComponentsWithEntityReadOnly<ViewComponent>();

        foreach (var targetEntity in m_EntityQuery.ForWorld(world.UID))
        {
            if (!behaviourComponents.TryGetComponent(targetEntity, out BehaviourComponent behaviourComponent))
                continue;

            viewComponents.TryGetComponent(targetEntity, out ViewComponent viewComponent);

            if (ECS.Core.GameObjectByRef.Exists(world.UID, viewComponent.ObjectReference))
                continue;

            viewComponent.ObjectReference = UpdateView(world, viewComponent, behaviourComponent);

            if (!viewComponent.IsReferenceIdValid)
                continue;

            AfterUpdateCommandBuffer.TryAddOrSetComponent(world.UID, targetEntity, viewComponent);
        }
    }

    int UpdateView(ECSWorld world, ViewComponent viewComponenent, BehaviourComponent behaviourComponent)
    {
        foreach (var setup in m_EntitySetup)
        {
            if (setup.Team != behaviourComponent.Team)
                continue;

            if (viewComponenent.IsReferenceIdValid)
            {
                ECS.Core.GameObjectByRef.GetOrReproduse(world.UID, setup.View, viewComponenent.ObjectReference);
                return viewComponenent.ObjectReference;
            }

            return ECS.Core.GameObjectByRef.Produse(world.UID, setup.View);
        }

        return ECS.Core.GameObjectUtils.InvalidReferenceId;
    }
}

[ECS.UnityProxy.ProxyComponent(ECS.UnityProxy.ProxyComponenentStateOverride.Exclude)]
public struct ViewComponent : IComponent
{
    public int ObjectReference;
    public bool IsReferenceIdValid => ECS.Core.GameObjectUtils.IsReferenceIdValid(ObjectReference);
    void IComponent.Register(in int world, in int entity) => ECS.Core.ComponentMap<ViewComponent>.TryAddOrSet(world, entity, this);
}



