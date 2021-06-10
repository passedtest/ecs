using ECS.Experimental.Collections;

namespace ECS.Experimental
{
    [System.Serializable]
    public struct ComponentWithCollectionPtr : IComponent
    {
        public CollectionPtr<int> IntCollection
        {
            get => CollectionPtr<int>.New(m_IntCollectionPtr);
            set => m_IntCollectionPtr = value;
        }

        //Collection pointer;
        [UnityEngine.SerializeField]
        EntityRangePtr m_IntCollectionPtr;

        void IComponent.Register(in int world, in int entity) => Core.ComponentMap<ComponentWithCollectionPtr>.TryAddOrSet(world, entity, this);
    }
}
