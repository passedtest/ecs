namespace ECS
{
    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Exclude)]
    public struct WorldTimeComponent : IComponent
    {
        public float Time;
        public int TickCount;

        void IComponent.Register(int world, int entity) => Core.ComponentMap<WorldTimeComponent>.TryAddOrSet(world, entity, this);
    }
}
