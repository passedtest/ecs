namespace ECS
{
    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Exclude)]
    public struct WorldTimeComponent : IComponent
    {
        public float Time;
        public int TickCount;

        void IComponent.Register(in int world, in int entity) => Core.ComponentMap<WorldTimeComponent>.TryAddOrSet(world, entity, this);
    }
}
