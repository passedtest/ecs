using ECS.Experimental.Collections;

namespace ECS.Experimental
{
    /// <summary>
    /// Demo component that shows, how collection storage may be implemented;
    /// </summary>
    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Exclude), RequireCollectionOfType(typeof(int))]
    public struct ComponentWithCollectionPtr : IComponent
    {
        public CollectionPtr<int> IntCollection
        {
            get => CollectionPtr<int>.New(m_IntCollectionPtr);
            set => m_IntCollectionPtr = value;
        }

        //Collection pointer, can be serialized;
        [UnityEngine.SerializeField]
        EntityRangePtr m_IntCollectionPtr;

        void IComponent.Register(in int world, in int entity) => Core.ComponentMap<ComponentWithCollectionPtr>.TryAddOrSet(world, entity, this);
    }
}
