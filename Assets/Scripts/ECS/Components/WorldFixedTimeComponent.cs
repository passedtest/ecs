namespace ECS
{

    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Exclude)]
    public struct WorldFixedTimeComponent : IComponent
    {
        public float FixedTime;
        public int FixedTickCount;

        void IComponent.Register(in int world, in int entity) => Core.ComponentMap<WorldFixedTimeComponent>.TryAddOrSet(world, entity, this);
    }
}
