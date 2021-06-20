using ECS.Core.Ptr;
using ECS.Core.Experimental.Collections;

namespace ECS.Core.Experimental
{
    /// <summary>
    /// Demo component that shows, how collection storage may be implemented;
    /// </summary>
    [System.Serializable, UnityProxy.ProxyComponent(UnityProxy.ProxyComponenentStateOverride.Exclude), RequireCollectionOfType(typeof(int))]
    public struct ComponentWithReferences : IComponent
    {
        public CollectionPtr<int> IntCollection
        {
            get => CollectionPtr<int>.New(m_IntCollectionPtr);
            set => m_IntCollectionPtr = value;
        }

        //Collection pointer, can be serialized;
        [UnityEngine.SerializeField]
        EntityRangePtr m_IntCollectionPtr;

        public ComponentPtr<TransformComponent> TransformComponent
        {
            get => ComponentPtr<TransformComponent>.New(m_TransformEntityPtr);
            set => m_TransformEntityPtr = value;
        }

        //Entity pointer, can be serialized;
        [UnityEngine.SerializeField]
        EntityPtr m_TransformEntityPtr;

        void IComponent.Register(in int world, in int entity) => ComponentMap<ComponentWithReferences>.TryAddOrSet(world, entity, this);
    }
}
